using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyAwesomeWebApi.Models.Identity;

namespace MyAwesomeWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure Identity MongoDB
            services.AddMongoIdentityProvider<ApplicationUser, ApplicationRole>
            (Configuration.GetConnectionString("MongoDbDatabase"), options =>             {                 options.Password.RequiredLength = 6;                 options.Password.RequireLowercase = true;                 options.Password.RequireUppercase = true;                 options.Password.RequireNonAlphanumeric = true;                 options.Password.RequireDigit = true;             });

            // Add Jwt Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims             services.AddAuthentication(options =>             {
                //Set default Authentication Schema as Bearer                 options.DefaultAuthenticateScheme =
                           JwtBearerDefaults.AuthenticationScheme;                 options.DefaultScheme =
                           JwtBearerDefaults.AuthenticationScheme;                 options.DefaultChallengeScheme =
                           JwtBearerDefaults.AuthenticationScheme;             }).AddJwtBearer(cfg =>             {                 cfg.RequireHttpsMetadata = false;                 cfg.SaveToken = true;                 cfg.TokenValidationParameters =
                       new TokenValidationParameters
                {                     ValidIssuer = Configuration["JwtIssuer"],                     ValidAudience = Configuration["JwtIssuer"],                     IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),                     ClockSkew = TimeSpan.Zero // remove delay of token when expire
                };             });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
