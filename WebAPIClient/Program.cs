using System;
using System.Threading.Tasks;

namespace WebAPIClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World from WebApiClient!");

            var test = new MyRestClient();
            await test.GetDataAsync(2, 4);
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}