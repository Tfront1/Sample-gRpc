using AuthorizationServer;
using AuthorizationServer.Protos;
using Grpc.Core;

namespace AuthorizationServer.Services
{
    public class AuthenticationService: Authentication.AuthenticationBase
    {
        public override async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request, ServerCallContext context)
        {
            var authenticationResponse = JwtAuthenticationManager.Authenticate(request) 
                ?? throw new RpcException(
                    new Status(StatusCode.Unauthenticated, "Invalid user info"));

            return authenticationResponse;
        }
    }
}
