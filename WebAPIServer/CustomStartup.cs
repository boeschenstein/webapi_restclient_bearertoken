﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebAPIBase;

namespace WebAPIServer
{
    internal static class StartupCustomExtensionsMethods
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add JwtSecret
            JwtSettings jwtSecret = new JwtSettings { Secret = "DO NOT TELL ANYONE", RSAPath = "" };
            byte[] key;
            if (!string.IsNullOrWhiteSpace(jwtSecret.RSAPath) && File.Exists(jwtSecret.RSAPath))
            {
                key = Encoding.ASCII.GetBytes(File.ReadAllText(jwtSecret.RSAPath));
            }
            else
            {
                key = Encoding.ASCII.GetBytes(jwtSecret.Secret);
            }

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,  // default = false
                        IssuerSigningKey = new SymmetricSecurityKey(key),

                        ValidIssuer = "Linux Client",
                        ValidateIssuer = true, // default = true

                        ValidAudience = "Job Center",
                        ValidateAudience = true, // default = true

                        //ValidateActor = false,  // default = true
                        //ValidateLifetime = false,  // default = true
                        //ValidateTokenReplay = false, // default = true
                    };

                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        },
                        OnForbidden = contex =>
                        {
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = contex =>
                        {
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = contex =>
                        {
                            Console.WriteLine($"---- OnTokenValidated Start ----");
                            Console.WriteLine($"Issuer: {contex.SecurityToken.Issuer}");
                            JwtSecurityToken jwt = (JwtSecurityToken)contex.SecurityToken;
                            foreach (Claim claim in jwt.Claims)
                            {
                                //Console.WriteLine($"Claim: {claim.Type}={claim.Value}");
                                if (claim.Type == "role")
                                {
                                    Console.WriteLine($"Role:{claim.Value}");
                                }
                            }
                            Console.WriteLine($"Subject: {jwt.Subject}");
                            foreach (string audience in jwt.Audiences)
                            {
                                Console.WriteLine($"Audience:{audience}");
                            }
                            Console.WriteLine($"---- OnTokenValidated End ----");

                            return Task.CompletedTask;
                        },
                        OnChallenge = contex =>
                        {
                            return Task.CompletedTask;
                        },
                    };
                });

            // configure DI for application services

            services.AddScoped<IUserResolver, UserResolver>();

            services.AddScoped<IUserService, JwtUserService>(sp => new JwtUserService(sp.GetService<IUserResolver>(), key, TimeSpan.FromHours(7.0)));

            return services;
        }
    }
}