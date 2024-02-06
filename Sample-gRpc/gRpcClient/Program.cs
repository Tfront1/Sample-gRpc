using Google.Protobuf.WellKnownTypes;
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

            /////////////////////////

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

            foreach (var product in productsGetResponse.Products) {
                var tempStockDate = product.StockDate.ToDateTime();
                Console.WriteLine($"{product.ProductCode} | {product.ProductName} | {product.Price} | {tempStockDate}");
            }

            await channel.ShutdownAsync();

            Console.ReadLine();
        }
    }
}