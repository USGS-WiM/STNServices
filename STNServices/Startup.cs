using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using STNDB;
using STNAgent;
using WiM.Security.Authentication.Basic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using STNServices.Codecs.CSV;
using WiM.Services.Analytics;
using WiM.Utilities.ServiceAgent;
using WiM.Services.Middleware;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.HttpOverrides;
using Amazon.S3;

namespace STNServices
{
    public class Startup
    {
        Dictionary<string, string> secrets = null;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            if (env.IsDevelopment()) {
                builder.AddUserSecrets<Startup>();
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
            
        }//end startup       

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            var awsoptions = Configuration.GetAWSOptions();
            IAmazonS3 client = awsoptions.CreateServiceClient<IAmazonS3>();

            services.AddScoped<IAnalyticsAgent, GoogleAnalyticsAgent>((gaa) => new GoogleAnalyticsAgent(Configuration["AnalyticsKey"]));
            // Add framework services.
            services.AddDbContext<STNDBContext>(options =>
                        options.UseNpgsql(String.Format(Configuration
                            .GetConnectionString("stnConnection"), Configuration["dbuser"], Configuration["dbpassword"], Configuration["dbHost"]),
                            //default is 1000, if > maxbatch, then EF will group requests in maxbatch size
                            opt => opt.MaxBatchSize(1000))
                            .EnableSensitiveDataLogging());

            services.AddScoped<ISTNServicesAgent, STNServicesAgent>(); //instead of this, trying ln56
            services.AddScoped<IBasicUserAgent, STNServicesAgent>();
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = BasicDefaults.AuthenticationScheme;
            }).AddBasicAuthentication();

            services.AddAuthorization(options => loadAutorizationPolicies(options));
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin()
                                                                 .AllowAnyMethod()
                                                                 .AllowAnyHeader()
                                                                 .AllowCredentials());
            });
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.OutputFormatters.Add(new STNCSVOutputFormater());
                options.FormatterMappings.SetMediaTypeMappingForFormat("csv", MediaTypeHeaderValue.Parse("text/csv"));
            })
                    .AddXmlSerializerFormatters()
                    .AddXmlDataContractSeria‌​lizerFormatters()
                    .AddJsonOptions(options => loadJsonOptions(options));

            // https://aws.amazon.com/blogs/developer/configuring-aws-sdk-with-net-core/
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>(); // doesn't work
        }

     

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            // global policy - assign here or on each controller
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
   
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.Use_Analytics();
            app.UseMvc();
            app.UseX_Messages();
        }

        #region Helper Methods

        private void loadAutorizationPolicies(AuthorizationOptions options)
        {           
            options.AddPolicy("CanModify", policy => policy.RequireRole("Admin", "Manager", "Field"));
            options.AddPolicy("Restricted", policy => policy.RequireRole("Admin", "Manager"));
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        }

        private void loadJsonOptions(MvcJsonOptions options)
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
            options.SerializerSettings.TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple;
            options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
        }
        private void ConfigureRoute(IRouteBuilder routeBuilder)
        {
            //login
            //routeBuilder.MapRoute("login", "{controller = Manager}/{action = GetLoggedInUser}");
        }
        #endregion


    }
}
