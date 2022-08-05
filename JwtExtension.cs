using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Trevisharp.Security.Jwt;

public static class JwtExtension
{
    public static void AddJwt(
        this WebApplicationBuilder builder, 
        int internalKeySize, TimeSpan updatePeriod)
    {
        builder.Services.AddSingleton<CryptoService>(p =>
        {
            var service = new CryptoService();
            service.InternalKeySize = internalKeySize;
            service.UpdatePeriod = updatePeriod;
            return service;
        });

        // builder.Services.AddSingleton<IAuthorizationHandler, JwtHandler>();

        // builder.Services.AddAuthorization(options =>
        // {
        //     options.AddPolicy("JwtToken", policy =>
        //     {
        //         policy.AddRequirements(new JwtRequirement());
        //     });
        // });
    }
}