using AuthorizationServer.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationServer.Services
{
    public class CalculationService: Calculation.CalculationBase
    {
        [Authorize(Roles = "Admin")]
        public override async Task<CalculationResult> Add(InputNumbers request, ServerCallContext context)
        {
            return new CalculationResult {
                Result = request.Number1 + request.Number2,
            };
        }

        [Authorize(Roles = "Admin,User")]
        public override async Task<CalculationResult> Substract(InputNumbers request, ServerCallContext context)
        {
            return new CalculationResult
            {
                Result = request.Number1 - request.Number2,
            };
        }
    }
}
