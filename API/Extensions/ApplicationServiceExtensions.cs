using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddScoped<ITokenService,TokenService>();
            services.AddScoped<IProductRepository,ProductRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IBasketRepository,BasketRepository>();
            services.AddScoped<IOrderService,OrderService>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actioncontext => 
                {
                    var errors = actioncontext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x=>x.Value.Errors)
                        .Select(x=>x.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationErrorResponse{
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}