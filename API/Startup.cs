using API.Errors;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddDbContext<StoreContext>(x => x.UseSqlite(_config.GetConnectionString("DefaultConnection")));
            services.AddDbContext<AppIdentityDbContext>(x => x.UseSqlite(_config.GetConnectionString("IdentityConnection")));

            services.AddSingleton<IConnectionMultiplexer>(c => 
            {
                var configuration = ConfigurationOptions.Parse(_config
                    .GetConnectionString("Redis"),
                    true);
                return ConnectionMultiplexer.Connect(configuration);
            });
            //extension method
            services.AddApplicationServices();
            services.AddIdentityServices(_config);

            services.AddAutoMapper(typeof(MappingProfiles));
            
            //extension method
            services.AddSwaggerDocumentation();
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>(); //use own own middleware to handle exception
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                //extension method
                app.UseSwaggerDocumentation();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();
            
            app.UseCors("CorsPolicy");
            
            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
