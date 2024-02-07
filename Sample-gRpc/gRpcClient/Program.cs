using AuthorizationServer.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Sample_Streaming_gRpc.Protos;
using SamplegRpc;
using System.Threading.Channels;

namespace gRpcClient
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            //var unaryChannel = GrpcChannel.ForAddress("http://localhost:5195");

            //await GetFullName(unaryChannel);

            //await AddGetProducts(unaryChannel);

            //await unaryChannel.ShutdownAsync();

            ////////////////////////

            //var streamingChannel = GrpcChannel.ForAddress("http://localhost:5014");

            //await ServerSampleStreaming(streamingChannel);

            //await ClientSampleStreaming(streamingChannel);

            //await BidirectionalSampleStreaming(streamingChannel);

            //await streamingChannel.ShutdownAsync();

            //////////////////////
            
            var authChannel = GrpcChannel.ForAddress("http://localhost:5094");

            string jwtToken = await GetJwtToken(authChannel);

            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {jwtToken}");
            await AddNumbers(authChannel, headers);

            await authChannel.ShutdownAsync();

            Console.ReadLine();
        }

        private static async Task GetFullName(GrpcChannel channel)
        {
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

        private async static Task ServerSampleStreaming(GrpcChannel channel)
        {
            var client = new SampleStream.SampleStreamClient(channel);

            var response = client.ServerSampleStreaming(new Test { TestMessage = "test" });

            while (await response.ResponseStream.MoveNext(CancellationToken.None))
            {
                var value = response.ResponseStream.Current.TestMessage;
                Console.WriteLine(value);
            }
            Console.WriteLine("Server streaming completed");
        }

        private async static Task ClientSampleStreaming(GrpcChannel channel)
        {
            Random random = new Random();

            var client = new SampleStream.SampleStreamClient(channel);

            var stream = client.ClientSampleStreaming();

            for (int i = 0; i < 5; i++)
            {
                await stream.RequestStream.WriteAsync(new Test { TestMessage = $"Message{i}" });
                await Task.Delay(random.Next(1, 10) * 1000);
            }
            //We need notify server that streaming completed
            await stream.RequestStream.CompleteAsync();
        }

        private async static Task BidirectionalSampleStreaming(GrpcChannel channel)
        {
            Random random = new Random();

            var client = new SampleStream.SampleStreamClient(channel);

            var stream = client.BidirectionalSampleStreaming();

            var requestTask = Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    await Task.Delay(random.Next(1, 10) * 1000);
                    await stream.RequestStream.WriteAsync(new Test { TestMessage = i.ToString() });
                    Console.WriteLine($"Sended request : {i}");
                }
                Console.WriteLine("Requesst strem compleeted");
                await stream.RequestStream.CompleteAsync();
            });

            var responseTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext(CancellationToken.None))
                {
                    Console.WriteLine($"Recieved responce : {stream.ResponseStream.Current.TestMessage}");
                }
                Console.WriteLine("Responce strem compleeted");
            });

            await Task.WhenAll(requestTask, responseTask);
        }

        private async static Task<string> GetJwtToken(GrpcChannel channel)
        {
            var authClient = new Authentication.AuthenticationClient(channel);

            var response = await authClient.AuthenticateAsync(new AuthenticationRequest
            {
                UserName = "admin",
                Password = "admin"
            });

            Console.WriteLine($"Auth token : {response.AccessToken}\nExpiresIn : {response.ExpiresIn}");
            return response.AccessToken;
        }

        private async static Task<int> AddNumbers(GrpcChannel channel, Metadata headers)
        {
            var calculateClient = new Calculation.CalculationClient(channel);

            var result = calculateClient.Add(new InputNumbers { Number1 = 1, Number2 = 2 }, headers);

            Console.WriteLine($"Adding result : {result.Result}");
            return result.Result;
        }
    }
}