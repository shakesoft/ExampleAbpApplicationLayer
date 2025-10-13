using System.Threading.Tasks;
using Volo.Abp.Account.Pro.Public.Blazor.Shared.Pages.Account.Idle;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Volo.Abp.Account.Pro.Public.Blazor.WebAssembly;

public class BlazorWebAssemblyAccountIdleCheckService : IAccountIdleCheckService , ITransientDependency
{
    private readonly ICurrentUser _currentUser;

    public BlazorWebAssemblyAccountIdleCheckService(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public Task<bool> IsEnabledAsync()
    {
        return Task.FromResult(_currentUser.IsAuthenticated);
    }
}