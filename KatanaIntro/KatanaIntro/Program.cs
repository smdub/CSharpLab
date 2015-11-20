using Owin;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KatanaIntro
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using System.IO;
    
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {           
            //app.UseWelcomePage();
            //app.Use(async (environment, next) =>
            //    {
            //        foreach (var pair in environment.Environment)
            //        {
            //            Console.WriteLine("{0}:{1}", pair.Key, pair.Value);
            //        }
            //        await next();
            //    });

            
            app.Use(async (environment, next) =>
            {
                Console.WriteLine("Requesting: {0}", environment.Request.Path);
                await next();
                Console.WriteLine("Response: {0}", environment.Response.StatusCode);
                
            });

            ConfigureWebApi(app);

            app.Run(ctx =>
            {
                Console.WriteLine("Request received from: {0}:{1} [{2}]", ctx.Request.RemoteIpAddress, ctx.Request.RemotePort, ctx.Request.Method);
                return ctx.Response.WriteAsync("Hello World!");
            });
        }
        public void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}", 
                new { id = RouteParameter.Optional });
            app.UseWebApi(config);
        }
    }


    public static class AppBuilderExtensions
    {
        public static void UseHelloWorld(this IAppBuilder app)
        {
            app.Use<HelloWorldComponent>();
        }
    }
    public class HelloWorldComponent
    {
        private readonly AppFunc _next;

        public HelloWorldComponent(AppFunc next)
        {
            _next = next;
        }
        public Task Invoke(IDictionary<string, object> environment)
        {
            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                return writer.WriteAsync("Hello World!");
            }
                
        }


    }
}
