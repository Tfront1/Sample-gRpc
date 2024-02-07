using AuthorizationServer.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationServer.Services
{
    [Authorize]
    public class CalculationService: Calculation.CalculationBase
    {
        public override async Task<CalculationResult> Add(InputNumbers request, ServerCallContext context)
        {
            return new CalculationResult {
                Result = request.Number1 + request.Number2,
            };
        }

        public override async Task<CalculationResult> Substract(InputNumbers request, ServerCallContext context)
        {
            return new CalculationResult
            {
                Result = request.Number1 - request.Number2,
            };
        }
    }
}
