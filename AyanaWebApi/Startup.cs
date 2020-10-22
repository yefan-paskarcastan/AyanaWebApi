using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using AyanaWebApi.Utils;
using AyanaWebApi.Models;
using AyanaWebApi.Services;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<AyDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AyanaDB")));

            var jwtSection = Configuration.GetSection("JWTSettings");
            services.Configure<JWTSettings>(jwtSection);

            var appSettings = jwtSection.Get<JWTSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);
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
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddHttpClient("torrentSoft", c =>
            {
                c.BaseAddress = new Uri("https://torrent-soft.net");
            });

            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRutorService, RutorService>();
            services.AddScoped<INnmclubService, NnmclubService>();
            services.AddScoped<ISoftService, SoftService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<IImghostService, ImghostService>();
            services.AddScoped<IImgsConverterService, ImgsConverterService>();
            services.AddScoped<IEveningWorkService, EveningWorkService>();
            services.AddScoped<ITegreiUIService, TegreiUIService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
