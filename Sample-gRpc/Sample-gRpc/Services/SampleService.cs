using Grpc.Core;
using SamplegRpc;

namespace Sample_gRpc.Services
{
    public class SampleService: Sample.SampleBase
    {
        public override async Task<SampleResponce> GetFullName(SampleRequest request, ServerCallContext context)
        {
            var result = $"{request.FirstName} {request.LastName}";
            return new SampleResponce { FullName = result };
        }
    }
}
