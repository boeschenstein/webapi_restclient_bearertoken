using System;
using System.Text;
using System.Threading.Tasks;
using WebAPIBase;

namespace WebAPIClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World from WebApiClient!");

            Console.WriteLine("Creating Token now...");

            const string mySecret = "DO NOT TELL ANYONE";
            IUserResolver userResolver = new UserResolver();

            byte[] secret = Encoding.ASCII.GetBytes(mySecret);
            TimeSpan expirationTime = new TimeSpan();
            JwtUserService userService = new JwtUserService(userResolver, secret, expirationTime);

            string username = "abba";
            string password = "waterloo";
            User user = await userService.AuthenticateAsync(username, password);

            Console.WriteLine("... Token generated.");

            Console.WriteLine("Accessing WebApi now...");
            var test = new MyRestClient();
            await test.GetDataAsync(user.Tokens.AuthToken);
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}