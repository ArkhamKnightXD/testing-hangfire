using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LearningHangFire
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Primero agrego el servicio de hangfire

            services.AddHangfire(configuration =>
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseDefaultTypeSerializer()
                    .UseMemoryStorage());

            //Lo siguiente es agregar el hangfire server
            services.AddHangfireServer();

            //Agrego servicio que trabajara con hangfire
            services.AddSingleton<IJobService, JobService>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //Se necesita agregar el ibackgroundjobclient para poder realizar tareas en colas en hangfire
        //Se necesita agregar el IRecurringJobManager para poder realizar tareas recurrentes en hangfire
        //ServiceProvider me permite utilizar 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            //Luego de agregado hangfire aqui me encargo de configurarlo
            
            //dashboard de hangfire
            app.UseHangfireDashboard();

            //creacion de una simple tarea en cola
          //  backgroundJobClient.Enqueue(() => Console.WriteLine("Hello World"));

            //Creacion de una tarea recurrente normal
           // recurringJobManager.AddOrUpdate("Run Every Minute", ()=> Console.WriteLine("Test"), "* * * * *");

            //Creacion de una tarea recurrente utilizando jobservice
            recurringJobManager.AddOrUpdate("Run Every Minute",() => serviceProvider.GetService<IJobService>().PrintJob(), "* * * * *");
        }
    }
}
