using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace WebAPIBase
{
    /// <summary>
    /// Implementation of the Interface <see cref="IUserService"/>
    /// This Implementation provides a UserService that validates the User and creates a JWT Token <see cref="https://jwt.io/"/>
    /// </summary>
    public class JwtUserService : IUserService
    {
        private readonly byte[] _secret;
        private TimeSpan _expirationTime;

        /// <inheritdoc cref="IUserService"/>
        public IUserResolver UserResolver { get; }

        /// <summary>
        /// The Constructor of this Service
        /// </summary>
        /// <param name="userResolver">Instance of <see cref="IUserResolver"/></param>
        /// <param name="secret">The Secret Key which will be used for Signing the JWT-Token</param>
        /// <param name="expirationTime">The Timespan which defines how long the created Token is Valid</param>
        public JwtUserService(IUserResolver userResolver, byte[] secret, TimeSpan expirationTime)
        {
            if (secret == null || !secret.Any())
            {
                throw new SvcSecurityInternalException("JwtUserService.ctor could not create Instance. Secret was null or empty!");
            }

            UserResolver = userResolver ?? throw new SvcSecurityInternalException("JwtUserService.ctor could not create Instance. UserResolver was null or empty!");
            _secret = secret;
            _expirationTime = expirationTime;
        }

        /// <inheritdoc cref="IUserService"/>
        public async Task<User> AuthenticateAsync(string username, string password, string audience, string issuer)
        {
            try
            {
                // The Resolver (DI) checks if the username and password are correct
                // If the combination is correct the User will be returned
                // This is the only appearance where we have to call the Resolver and the external or internal user store
                var user = await UserResolver.ResolveAsync(username, password);

                if (user == null)
                {
                    return null;
                }

                //Reset Password to empty
                user.Password = string.Empty;

                // Create the Tokenhandler and the private Key from the AppSettings
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = _secret;

                // Create the Claim with the Username and the Roles
                var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.Username) };
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                foreach (var svcRole in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, svcRole.RoleName));
                }

                var exp = _expirationTime != TimeSpan.Zero ? DateTime.UtcNow + _expirationTime : DateTime.UtcNow.AddDays(1);

                // create the TokenDescriptor with the Claims, the expiration duration and the SingningCredentials
                // in this example we use a Symmetric algorithm, with an central external auth service a asymmetric algorithm could be the better choice
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = exp,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Audience = audience,
                    Issuer = issuer
                };
                // Create and writ new JWT Token
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Tokens = new Tokens();
                user.Tokens.AuthToken = tokenHandler.WriteToken(token);

                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}