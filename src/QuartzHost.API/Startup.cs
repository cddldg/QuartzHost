using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Text.Encodings.Web;
using DG.Dapper;
using DG.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using QuartzHost.API.Common;
using QuartzHost.Core.Common;
using QuartzHost.Core.Models;
using QuartzHost.Core.Services;

namespace QuartzHost.API
{
    public class Startup
    {
        private ILogger logger = DG.Logger.DGLogManager.GetLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var nodeSetting = Configuration.GetSection("NodeSetting").Get<NodeSetting>();
            var sqlConnString = Configuration.GetConnectionString(nodeSetting.DbType);
            //加载注入DbContex
            if (nodeSetting.DbType == "Sqlite")
            {
                sqlConnString = $"{sqlConnString.Split("=")[0]}={Path.Combine(AppContext.BaseDirectory, "App_Data", sqlConnString.Split("=")[1])}";
                services.AddSql(SQLiteFactory.Instance, sqlConnString);
            }

            if (nodeSetting.DbType == "Mssql")
                services.AddSql(SqlClientFactory.Instance, sqlConnString);

            //加载注入配置
            nodeSetting.ConnStr = sqlConnString;
            services.AddSingleton(nodeSetting);

            //加载注入DGLogger
            services.AddLogging(x => x.AddDGLog());

            //注册所有业务service
            services.AddAppServices();
            //json
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });

            #region Swagger

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "QuartzHost API",
                        Description = "This is Quart.Net Host on .Net Core With Asp.Net Core3.1、Dapper2.0、MSSQL、SQLite 、Quartz.Net、ASP.NET Core Blazor",
                        TermsOfService = new Uri("https://github.com/cddldg/QuartzHost"),
                    });

                    #region Authorization

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "格式:Bearer {AccessToken}",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{ }
                    }
                    });
                    string filepath = Path.Combine(AppContext.BaseDirectory, "App_Data", "QuartzHost.API.xml");
                    c.IncludeXmlComments(filepath);

                    #endregion Authorization
                });

            #endregion Swagger

            #region Filter

            services.AddControllersWithViews(config =>
                {
                    //添加全局过滤器
                    config.Filters.Add(typeof(SimpleCheckAuthorization));
                    config.Filters.Add(typeof(GlobalExceptionFilter));
                });

            #endregion Filter

            #region ModelState

            services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.InvalidModelStateResponseFactory =
                        context => throw new BusinessException(context.ModelState);
                });

            #endregion ModelState
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="lifetime"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //静态资源
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuartzHost API V1");
            });

            app.UseRouting();

            app.UseAuthorization();
            //app.UseMvc();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var quartzService = app.ApplicationServices.GetService<IQuartzService>();
            quartzService.InitScheduler().Wait();
            quartzService.Start<Core.Common.TaskClearJob>("task-clear", "0 0/1 * * * ? *").Wait();
            lifetime.ApplicationStopping.Register(() =>
            {
                quartzService.Shutdown(true).Wait();
            });
        }
    }
}