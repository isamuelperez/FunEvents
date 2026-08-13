using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Infrastructure.Data;
using FunEvents.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Infrastructure
{
    public static class DependencyInjection
    {
        /*
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
        {
            services.AddDbContext<FunEventsDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }
        */

        public static IServiceCollection AddInfrastructure(
       this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
