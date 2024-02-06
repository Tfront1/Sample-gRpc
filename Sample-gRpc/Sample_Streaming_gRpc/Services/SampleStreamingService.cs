using Grpc.Core;
using Sample_Streaming_gRpc.Protos;

namespace Sample_Streaming_gRpc.Services
{
    public class SampleStreamingService: SampleStream.SampleStreamBase
    {
        private readonly Random random;
        public SampleStreamingService()
        {
            this.random = new Random();
        }

        public override async Task ServerSampleStreaming(Test request, IServerStreamWriter<Test> responseStream, ServerCallContext context)
        {
            for (int i = 0; i < 5; i++)
            {
                await responseStream.WriteAsync(new Test() { TestMessage = $"Message {i}"});
                await Task.Delay(random.Next(1,10) * 1000);
            }
        }

        public override async Task<Test> ClientSampleStreaming(IAsyncStreamReader<Test> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext()) 
            {
                Console.WriteLine(requestStream.Current.TestMessage);
            }
            Console.WriteLine("Client streaming complated");

            return new Test { TestMessage = "test" };
        }
    }

}
