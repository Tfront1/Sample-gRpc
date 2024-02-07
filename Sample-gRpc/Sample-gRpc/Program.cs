using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sample_gRpc.Services;
using System.Text;

namespace Sample_gRpc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("oX1V8vR/nwnFV5PArE+SG+gb3rVvqRiJ37RnYDCJ3Os=")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddGrpc();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();
            // Configure the HTTP request pipeline.
            app.MapGrpcService<SampleService>();
            app.MapGrpcService<ProductService>();

            app.Run();
        }
    }
}