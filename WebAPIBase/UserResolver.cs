using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPIBase
{
    public class UserResolver : IUserResolver
    {
        public async Task<User> ResolveAsync(string username, string password)
        {
            var user = new User { Username = username, Password = password, Roles = new List<Role> { new Role { RoleName = "TestRole1" }, new Role { RoleName = "TestRole2" } } };
            return await Task.FromResult(user);
        }
    }
}