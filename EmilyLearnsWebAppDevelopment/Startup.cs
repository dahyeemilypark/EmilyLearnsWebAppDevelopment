using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using EmilyLearnsWebAppDevelopment.Models;
using EmilyLearnsWebAppDevelopment.Services;
using EmilyLearnsWebAppDevelopment.Filters;

namespace EmilyLearnsWebAppDevelopment
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
            services.AddSingleton<ICreationData, CreationData>(); // Module 1
            services.AddSingleton<IPollResultsService, PollResultsService>(); // Module 3
            services.AddMvc(); // Module 3
            services.AddSingleton<ICityData, CityData>(); // Module 4
            //services.AddScoped<CityLogActionFilterAttribute>(); // Module 4
            services.AddSingleton<ICatProvider, CatProvider>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env) // Saving this in case something breaks later
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }
        //    else
        //    {
        //        app.UseExceptionHandler("/Home/Error");
        //    }
        //    app.UseStaticFiles();

        //    app.UseRouting();

        //    app.UseAuthorization();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllerRoute(
        //            name: "default",
        //            pattern: "{controller=Home}/{action=Index}/{id?}");
        //    });
        //}

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IPollResultsService pollResults) // Module 3
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Query.ContainsKey("favorite"))
                {
                    string selectedValue = context.Request.Query["favorite"];
                    SelectedColor selectedColor = (SelectedColor)Enum.Parse(typeof(SelectedColor), selectedValue, true);
                    pollResults.AddVote(selectedColor);
                    //SortedDictionary<SelectedColor, int> colorVotes = pollResults.GetVoteResult();

                    //foreach (KeyValuePair<SelectedColor, int> currentVote in colorVotes)
                    //{
                    //    await context.Response.WriteAsync($"<div> Color name: {currentVote.Key}. Votes: {currentVote.Value} </div>");
                    //}
                    context.Response.Headers.Add("content-type", "text/html");
                    await context.Response.WriteAsync("Thank you for submitting the poll. You may look at the poll results <a href='/Poll/?submitted=true'>Here</a>.");
                }
                else
                {
                    await next.Invoke();
                }
            }); // Module 3

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            //app.UseMvcWithDefaultRoute(); // Module 3

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //         name: "TravelerRoute",
            //         template: "{controller}/{action}/{name}",
            //         constraints: new { name = "[A-Za-z ]+" },
            //         defaults: new { controller = "Traveler", action = "Index", name = "Katie Bruce" });

            //    routes.MapRoute(
            //        name: "defaultRoute",
            //        template: "{controller}/{action}/{id?}",
            //        defaults: new { controller = "Home", action = "Index" },
            //        constraints: new { id = "[0-9]+" });
            //}); // Module 4

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
