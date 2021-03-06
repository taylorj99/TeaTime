﻿namespace TeaTime
{
    using AutoMapper;
    using Common;
    using Common.Abstractions;
    using Common.Features.Runs;
    using Common.Permissions;
    using Common.Services;
    using Data.MySql;
    using MediatR;
    using MediatR.Pipeline;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Slack;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var mvcBuilder = services.AddMvc();

            //TODO: Allow these to be loaded dynamically
            services.AddMySql(Configuration.GetConnectionString("mysql"));
            services.AddSlack(mvcBuilder, Configuration.GetSection("slack").Get<SlackOptions>());

            services.AddSingleton<IRunnerRandomizer, DefaultRunnerRandomizer>();
            services.AddSingleton<IRoomRunLockService, RoomRunLockService>();
            services.AddSingleton<ISystemClock, DefaultSystemClock>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddSingleton<IPermissionService, PermissionService>();

            services.AddMediatR(typeof(ICommand));
            services.AddAutoMapper(typeof(ICommand));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));

            services.AddSingleton<RunLockProcessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMvc();
        }
    }
}
