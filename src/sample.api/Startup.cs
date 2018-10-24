using System;
using System.Net.Http;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace sample.api
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
            var useDb = Configuration.GetValue<bool>("UseDb");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddFluentValidation();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ctx => new ValidationProblemDetailsResult();
            });

            services.AddSingleton<HttpClient>();
            services.AddSingleton<GithubClient>();

            services.AddHttpClient<UnreliableEndpointCallerService>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            services.AddTransient<IValidator<Lecture>, LectureValidator>();
            //services.AddHttpClient<GithutClient>();
            services.AddHttpClient("configured-inner-handler")
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        return new HttpClientHandler()
                        {
                            UseCookies = false,
                            AllowAutoRedirect = false,
                            UseDefaultCredentials = true
                        };
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
