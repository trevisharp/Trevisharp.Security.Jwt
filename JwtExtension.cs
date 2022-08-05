using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Trevisharp.Security.Jwt;

public static class JwtExtension
{
    public static void AddJwt(
        this IServiceCollection services, 
        int internalKeySize, TimeSpan updatePeriod)
    {
        services.AddSingleton<CryptoService>(p =>
        {
            var service = new CryptoService();
            service.InternalKeySize = internalKeySize;
            service.UpdatePeriod = updatePeriod;
            return service;
        });

        services.AddSingleton<IAuthorizationHandler, JwtHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("JwtToken", policy =>
            {
                policy.AddRequirements(new JwtRequirement());
            });
        });
    }
}