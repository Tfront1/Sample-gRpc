using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Sample_Streaming_gRpc.Protos;
using SamplegRpc;

namespace gRpcClient
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5195");

            await GetFullName(channel);

            await AddGetProducts(channel);

            await channel.ShutdownAsync();

            await ServerSampleStreaming();

            Console.ReadLine();
        }

        private static async Task GetFullName(GrpcChannel channel) {
            var client = new Sample.SampleClient(channel);

            //var responce = client.GetFullName(new SampleRequest { FirstName = "Taras", LastName = "Taranko" });
            //OR
            var responce = await client.GetFullNameAsync(new SampleRequest { FirstName = "Taras", LastName = "Taranko" });

            Console.WriteLine(responce.FullName);
        }

        private static async Task AddGetProducts(GrpcChannel channel)
        {
            var productClient = new Product.ProductClient(channel);


            var stockDate = DateTime.SpecifyKind(new DateTime(2022, 1, 1), DateTimeKind.Utc);
            var productAddResponse = await productClient.AddProductAsync(new ProductModel
            {
                ProductCode = "1",
                ProductName = "Bim",
                Price = 1000,
                StockDate = Timestamp.FromDateTime(stockDate)
            }); ;
            Console.WriteLine($"{productAddResponse.StatusCode} | {productAddResponse.IsSuccessful}");

            var productsGetResponse = await productClient.GetAllProductsAsync(new Google.Protobuf.WellKnownTypes.Empty());

            foreach (var product in productsGetResponse.Products)
            {
                var tempStockDate = product.StockDate.ToDateTime();
                Console.WriteLine($"{product.ProductCode} | {product.ProductName} | {product.Price} | {tempStockDate}");
            }
        }

        private async static Task ServerSampleStreaming()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5014");

            var client = new SampleStream.SampleStreamClient(channel);

            var response = client.ServerSampleStreaming(new Test {TestMessage = "test" });

            while (await response.ResponseStream.MoveNext(CancellationToken.None))
            {
                var value = response.ResponseStream.Current.TestMessage;
                Console.WriteLine(value);
            }
            Console.WriteLine("Server streaming completed");

            await channel.ShutdownAsync();
        }
    }
}