using Grpc.Net.Client;
using SamplegRpc;

namespace gRpcClient
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5195");

            var client = new Sample.SampleClient(channel);

            //var responce = client.GetFullName(new SampleRequest { FirstName = "Taras", LastName = "Taranko" });
            //OR
            var responce = await client.GetFullNameAsync(new SampleRequest { FirstName = "Taras", LastName = "Taranko" });

            Console.WriteLine(responce.FullName);

            Console.ReadLine();
        }
    }
}