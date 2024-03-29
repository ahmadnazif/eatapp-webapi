﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EatAppApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EatAppApi
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
            services.AddTransient<ITimezoneHelper, TimezoneHelper>();
            services.AddSingleton<IMysqlDbHelper, MysqlDbHelper>();

            // new
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
                c.AddPolicy("AllowAllHeaders", options => options.AllowAnyHeader());
                c.AddPolicy("AllowAllMethods", options => options.AllowAnyMethod());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
                options.AllowAnyHeader();
            }); // new
            app.UseMvc();
        }
    }
}
