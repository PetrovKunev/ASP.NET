﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using CinemaApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Web.Infrastructure.Extensions
{
    public static class ExtensionMethods
    {
        public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
        {
          using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

            CinemaDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<CinemaDbContext>()!;
            dbContext.Database.Migrate();

            return app;
        }
    }
}
