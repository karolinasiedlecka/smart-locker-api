using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartLocker.Application.Interfaces;
using SmartLocker.Application.Services;
using SmartLocker.Infrastructure.Persistence;

namespace SmartLocker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureAndApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Brak connection stringa 'DefaultConnection'.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ISmartLockerStore, SmartLockerStore>();
        services.AddScoped<IParcelService, ParcelService>();
        services.AddScoped<ILockerService, LockerService>();

        return services;
    }
}