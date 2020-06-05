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
            IUserResolver userResolver = new UserResolver(); // todo: DependencyInjection in .NET Core Console App ? (add AutoFac ?)

            byte[] secret = Encoding.ASCII.GetBytes(mySecret);
            TimeSpan expirationTime = TimeSpan.FromHours(7.0);
            JwtUserService userService = new JwtUserService(userResolver, secret, expirationTime);

            string username = @"myDomain\myUser";
            string password = "<no password needed here>";
            string audience = "Job Center";
            string issuer = "Linux Client";
            User user = await userService.AuthenticateAsync(username, password, audience, issuer);

            Console.WriteLine("... Token generated.");

            Console.WriteLine("Accessing WebApi (and adding Token to Header) now...");
            var test = new MyRestClient();
            await test.GetDataAsync(user.Tokens.AuthToken);
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}