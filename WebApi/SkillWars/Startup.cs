using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Serilog;
using Serilog.Events;
using SkillWars.Extentions;
using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Services.LoginService;
using Microsoft.CodeAnalysis;
using Common.Interfaces.Services;
using Services.AccountService;
using Services.SendingServices;
using Services.TimeredFunctionsService;
using Services.LobbieService;
using WebSocketLayer.General.Interfaces;
using WebSocketLayer.SocketServer;

namespace SkillWars
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                 .AddJsonFile($"appsettings.Development.json", optional: true)
                 .AddJsonFile($"EmailNotificationsLocalization.json", optional: true)
                 .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "SkillWars API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",
                });
            });

            services.AddTransient(_ => Configuration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,

                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,

                        ValidateLifetime = true,

                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddDbContext<MSContext>(
                options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("DataAccessLayer"));
                });

            services.AddCors();

            services.AddSingleton<ISkillWarsServer, SkillWarsServer>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ILobbieService, LobbieService>();
            services.AddTransient<ITimeredFunctionsService, TimeredFunctionsService>();

            services.AddMvc();
        }

        
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, MSContext context, ITimeredFunctionsService timeredService, ISkillWarsServer serv)
        {
            loggerFactory.AddConsole();
            SetUpLogger(env, loggerFactory);
                      

            app.UseExceptionHandlerMiddleware();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            await timeredService.Setup();
            //EnsureDataBaseReady(context);
            app.UseAuthentication();
            //app.UseDefaultFiles();
            //app.UseStaticFiles();
            
            app.UseSwagger();           
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            
            app.UseMvc();
        }

        private void SetUpLogger(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {            
            var logPath = Path.Combine(hostingEnvironment.ContentRootPath, "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            Serilog.Core.Logger logger;

            logger = new LoggerConfiguration().WriteTo.Logger(l => l.Filter
                    .ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo
                    .RollingFile(@"Logs\Info-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo
                    .RollingFile(@"Logs\Debug-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo
                    .RollingFile(@"Logs\Warning-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo
                    .RollingFile(@"Logs\Error-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo
                    .RollingFile(@"Logs\Fatal-{Date}.log")).CreateLogger();

            loggerFactory.AddSerilog(logger);
        }

        private void EnsureDataBaseReady(MSContext context)
        {
            try
            {
                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
            catch 
            {
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
        }
    }
}
