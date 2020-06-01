using System.Collections.Generic;

namespace WebAPIBase
{
    public class User
    {
        public User()
        {
            Roles = new List<Role>();
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public List<Role> Roles { get; set; }
        public Tokens Tokens { get; set; }
    }
}