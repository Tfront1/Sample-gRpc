using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SamplegRpc;

namespace Sample_gRpc.Services
{
    public class ProductService : Product.ProductBase
    {
        public override async Task<ProductAddResponse> AddProduct(ProductModel request, ServerCallContext context)
        {
            Console.WriteLine($"{request.ProductCode} | {request.ProductName} | {request.Price} | {request.StockDate}");

            var result = new ProductAddResponse() {
                StatusCode = 201,
                IsSuccessful = true 
            };

            return result;
        }

        public override async Task<ProductList> GetAllProducts(Empty request, ServerCallContext context)
        {
            var stockDate = DateTime.SpecifyKind(new DateTime(2022, 1, 1), DateTimeKind.Utc);
            var product1 = new ProductModel()
            {
                ProductCode = "1",
                ProductName = "Nim",
                Price = 1001,
                StockDate = Timestamp.FromDateTime(stockDate),
            };
            var product2 = new ProductModel()
            {
                ProductCode = "2",
                ProductName = "Him",
                Price = 1002,
                StockDate = Timestamp.FromDateTime(stockDate),
            };
            var product3 = new ProductModel()
            {
                ProductCode = "3",
                ProductName = "Mim",
                Price = 1003,
                StockDate = Timestamp.FromDateTime(stockDate),
            };

            var result = new ProductList();
            result.Products.Add(product1);
            result.Products.Add(product2);
            result.Products.Add(product3);

            return result;
        }
    }
}
