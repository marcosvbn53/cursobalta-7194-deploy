using Shop;
using Shop.Data;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Show
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var host =  CreateHostBuilder(args)
            .Build();

            //CreateDbIfNotExistis(host);
            
            host.Run();            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        // public static void CreateDbIfNotExistis(IHost host)                
        // {
        //     using(var scope = host.Services.CreateScope()){
        //         var services = scope.ServiceProvider;
        //         try
        //         {
        //             var context = services.GetRequiredService<DataContext>();
        //             context.Database.EnsureCreated();

        //         }catch(Exception ex){
        //             //var logger = services.Get
        //         }
        //     }
        // }
    }
}
