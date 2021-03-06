using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LiteWebSocket;
using Microsoft.AspNetCore.Http;
using Example.Filters;
using LiteWebSocket.Routing;

namespace Example
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
            services.AddMvc();

            services.AddLiteWebSocket();

            services.AddTransient<TestController>();//TODO: add register all from assembly
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseLiteWebSocket(new LiteWebSocketOptions() { BasePath = new PathString("/test/lws") });

            MessageControllerResolver resolver = app.ApplicationServices.GetService<MessageControllerResolver>();
            resolver.AddSupportedMessage<TestMessageScope.Test1>();
            resolver.AddSupportedMessage<TestMessageScope.Test2>();
            resolver.AddSupportedMessage<TestMessageScope.Test3>();
            resolver.RegisterController<TestController>();

            LiteWebSocketDefaults.Filters.Add(new GlobalFilter());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
