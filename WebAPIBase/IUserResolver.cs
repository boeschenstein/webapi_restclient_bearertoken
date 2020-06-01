using System.Threading.Tasks;

namespace WebAPIBase
{
    public interface IUserResolver
    {
        Task<User> ResolveAsync(string username, string password);
    }
}