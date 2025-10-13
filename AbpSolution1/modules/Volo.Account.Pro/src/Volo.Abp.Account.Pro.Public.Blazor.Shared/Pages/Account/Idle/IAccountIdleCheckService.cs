using System.Threading.Tasks;

namespace Volo.Abp.Account.Pro.Public.Blazor.Shared.Pages.Account.Idle;

public interface IAccountIdleCheckService 
{
    Task<bool> IsEnabledAsync();
}