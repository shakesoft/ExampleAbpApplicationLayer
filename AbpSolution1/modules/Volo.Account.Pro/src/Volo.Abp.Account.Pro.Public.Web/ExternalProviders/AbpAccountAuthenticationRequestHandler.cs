using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Account.Public.Web.ExternalProviders;

public class AbpAccountAuthenticationRequestHandler<TOptions, THandler> : IAuthenticationRequestHandler, IAuthenticationSignInHandler
    where TOptions : RemoteAuthenticationOptions, new()
    where THandler : RemoteAuthenticationHandler<TOptions>
{
    protected readonly THandler InnerHandler;
    protected readonly IOptions<TOptions> OptionsManager;
    protected readonly ICurrentTenant CurrentTenant;
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly ILogger<AbpAccountAuthenticationRequestHandler<TOptions, THandler>> Logger;

    public AbpAccountAuthenticationRequestHandler(
        THandler innerHandler,
        IOptions<TOptions> optionsManager,
        ICurrentTenant currentTenant,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AbpAccountAuthenticationRequestHandler<TOptions, THandler>> logger)
    {
        InnerHandler = innerHandler;
        OptionsManager = optionsManager;
        CurrentTenant = currentTenant;
        HttpContextAccessor = httpContextAccessor;
        Logger = logger;
    }

    public virtual async Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        await InnerHandler.InitializeAsync(scheme, context);
    }

    public virtual async Task<AuthenticateResult> AuthenticateAsync()
    {
        return await InnerHandler.AuthenticateAsync();
    }

    public virtual async Task ChallengeAsync(AuthenticationProperties properties)
    {
        await TryAddTenantInfoAsync(properties);
        await SetOptionsAsync();

        await InnerHandler.ChallengeAsync(properties);
    }

    public virtual async Task ForbidAsync(AuthenticationProperties properties)
    {
        await TryAddTenantInfoAsync(properties);

        await InnerHandler.ForbidAsync(properties);
    }

    public async Task SignOutAsync(AuthenticationProperties properties)
    {
        await TryAddTenantInfoAsync(properties);

        var signOutHandler = InnerHandler as IAuthenticationSignOutHandler;
        if (signOutHandler == null)
        {
            throw new InvalidOperationException($"The authentication handler registered for scheme '{InnerHandler.Scheme}' is '{InnerHandler.GetType().Name}' which cannot be used for SignOutAsync");
        }

        await SetOptionsAsync();
        await signOutHandler.SignOutAsync(properties);
    }

    public async Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
    {
        await TryAddTenantInfoAsync(properties);

        var signInHandler = InnerHandler as IAuthenticationSignInHandler;
        if (signInHandler == null)
        {
            throw new InvalidOperationException($"The authentication handler registered for scheme '{InnerHandler.Scheme}' is '{InnerHandler.GetType().Name}' which cannot be used for SignInAsync");
        }

        await SetOptionsAsync();
        await signInHandler.SignInAsync(user, properties);
    }

    public virtual async Task<bool> HandleRequestAsync()
    {
        var tenantId = CurrentTenant.Id ?? await GetTenantIdOrNullAsync();
        using (CurrentTenant.Change(tenantId))
        {
            if (await InnerHandler.ShouldHandleRequestAsync())
            {
                await SetOptionsAsync();
            }

            return await InnerHandler.HandleRequestAsync();
        }
    }

    public virtual THandler GetHandler()
    {
        return InnerHandler;
    }

    protected virtual async Task SetOptionsAsync()
    {
        await OptionsManager.SetAsync(InnerHandler.Scheme.Name);
        ObjectHelper.TrySetProperty(InnerHandler, handler => handler.Options, () => OptionsManager.Value);
    }

    protected virtual Task TryAddTenantInfoAsync(AuthenticationProperties properties)
    {
        if (CurrentTenant.Id.HasValue)
        {
            properties?.Items.TryAdd("TenantId", CurrentTenant.Id?.ToString());
        }
        return Task.CompletedTask;
    }

    protected virtual async Task<Guid?> GetTenantIdOrNullAsync()
    {
        if (!await InnerHandler.ShouldHandleRequestAsync())
        {
            return null;
        }

        if (HttpContextAccessor.HttpContext == null)
        {
            return null;
        }

        var state = HttpContextAccessor.HttpContext.Request.Query["state"];
        if (state.IsNullOrEmpty() && HttpContextAccessor.HttpContext.Request.HasFormContentType)
        {
            var formCollection = await HttpContextAccessor.HttpContext.Request.ReadFormAsync();
            state = formCollection["state"];
        }

        if (state.IsNullOrEmpty())
        {
            return null;
        }

        try
        {
            var secureDataFormat = await GetSecureDataFormatOrNullAsync();
            if (secureDataFormat == null)
            {
                return null;
            }

            var authenticationProperties = secureDataFormat.Unprotect(state);
            if (authenticationProperties == null)
            {
                return null;
            }

            var tenantId = authenticationProperties.Items.FirstOrDefault(x => x.Key == "TenantId");
            if (tenantId.Value.IsNullOrEmpty())
            {
                return null;
            }

            if (!Guid.TryParse(tenantId.Value, out var tenantIdGuid))
            {
                return null;
            }

            return tenantIdGuid;
        }
        catch (Exception e)
        {
            Logger.LogWarning(e, "Could not get tenant id from the state.");
            return null;
        }
    }

    protected readonly ConcurrentDictionary<Type, PropertyInfo> StateDataFormatPropertyInfoCache = new ConcurrentDictionary<Type, PropertyInfo>();

    protected virtual Task<ISecureDataFormat<AuthenticationProperties>> GetSecureDataFormatOrNullAsync()
    {
        var propertyInfo = StateDataFormatPropertyInfoCache.GetOrAdd(InnerHandler.Options.GetType(),
            type => type.GetProperty("StateDataFormat",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
        if (propertyInfo != null)
        {
            var secureDataFormat = propertyInfo.GetValue(InnerHandler.Options) as ISecureDataFormat<AuthenticationProperties>;
            return Task.FromResult(secureDataFormat);
        }

        return Task.FromResult<ISecureDataFormat<AuthenticationProperties>>(null);
    }
}
