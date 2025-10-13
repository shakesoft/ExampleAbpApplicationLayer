using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Account.Pro.Public.Web.Shared.Pages.Account.Idle;

public class AccountIdleWebCheckService : ITransientDependency
{
    public virtual Task<bool> IsEnabledAsync(HttpContext httpContext)
    {
        var user = httpContext.User;

        // Disable idle check for anonymous users.
        if (user.Identity == null || user.Identity.IsAuthenticated != true)
        {
            return Task.FromResult(false);
        }

        // Disable idle check for persistent logins.
        var authenticateResultFeature = httpContext.Features.Get<IAuthenticateResultFeature>();
        var isPersistent = authenticateResultFeature?.AuthenticateResult?.Properties?.IsPersistent ?? false;
        if (isPersistent)
        {
            return Task.FromResult(false);
        }

        // Check user.Identity.AuthenticationType or something else here to disable idle check for some case.

        return Task.FromResult(true);
    }
}
