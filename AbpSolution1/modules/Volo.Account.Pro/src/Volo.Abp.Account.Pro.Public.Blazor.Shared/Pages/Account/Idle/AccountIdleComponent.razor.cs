using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Volo.Abp.Account.Localization;
using Volo.Abp.Account.Settings;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Web.Configuration;
using Volo.Abp.AspNetCore.Components.Web.Security;
using Volo.Abp.Settings;
using Volo.Abp.Threading;

namespace Volo.Abp.Account.Pro.Public.Blazor.Shared.Pages.Account.Idle;

public partial class AccountIdleComponent : IAsyncDisposable
{
    [Inject]
    protected IOptions<AbpAuthenticationOptions> AuthenticationOptions { get; set; }
    
    [Inject]
    protected IOptions<AbpAspNetCoreComponentsWebOptions> AspNetCoreComponentsWebOptions { get; set; }
    
    [Inject]
    protected ISettingProvider SettingProvider { get; set; }
    
    [Inject]
    protected AbpIdleTrackerService<AccountIdleComponent> IdleTrackerService { get; set; }
    
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected AbpAsyncTimer SignOutTimer { get; set; }
    
    [Inject]
    protected IAccountIdleCheckService IAccountIdleCheckService { get; set; }
    
    [Inject]
    protected ApplicationConfigurationChangedService ApplicationConfigurationChangedService { get; set; }
    
    [Inject]
    protected ICurrentApplicationConfigurationCacheResetService CurrentApplicationConfigurationCacheResetService { get; set; }
    
    protected DotNetObjectReference<AccountIdleComponent> DotNetObjectRef { get; set; }
    
    protected Modal ModalRef { get; set; }
    
    protected int IdleTimeoutMinutes { get; set; }
    
    protected bool IsEnabled { get; set; }
    
    protected string SignOutText { get; set; }

    protected int CountDown { get; set; } = 60;
    
    protected bool IsAccountIdleTrackerCreated { get; set; }

    public AccountIdleComponent()
    {
        LocalizationResource = typeof(AccountResource);
    }
    
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitializeAccountIdleTrackerAsync();
            await CreateAccountIdleTrackerAsync();
            ApplicationConfigurationChangedService.Changed += ApplicationConfigurationChanged;
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }
    
    protected virtual async Task InitializeAccountIdleTrackerAsync()
    {
        DotNetObjectRef = DotNetObjectReference.Create(this);
        await IdleTrackerService.InitializeAsync(DotNetObjectRef, nameof(OnAccountIdleSettingsChangedAsync));
    }

    protected virtual async Task CreateAccountIdleTrackerAsync()
    {
        if (IsAccountIdleTrackerCreated)
        {
            await ResetIdleTrackerAsync();
            await IdleTrackerService.DisposeAsync();
        }

        IsAccountIdleTrackerCreated = false;
        if (!await IAccountIdleCheckService.IsEnabledAsync())
        {
            return;
        }
        
        IsEnabled = await SettingProvider.GetAsync<bool>(AccountSettingNames.Idle.Enabled);
        if (!IsEnabled)
        {
            return;
        }

        SignOutText = L["IdleSignOutNow"];
        IdleTimeoutMinutes = await SettingProvider.GetAsync<int>(AccountSettingNames.Idle.IdleTimeoutMinutes);

        await IdleTrackerService.CreateAsync(IdleTimeoutMinutes * 60 * 1000, nameof(OnIdleCallbackAsync), nameof(OnStorageCallbackAsync));
        await IdleTrackerService.StartAsync();
        await IdleTrackerService.SyncStateAsync();

        IsAccountIdleTrackerCreated = true;
        
        await InvokeAsync(StateHasChanged);
    }
    
    protected virtual async void ApplicationConfigurationChanged()
    {
        try
        {
            await CreateAccountIdleTrackerAsync();
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }
    
    [JSInvokable]
    public virtual async Task OnAccountIdleSettingsChangedAsync()
    {
        await CurrentApplicationConfigurationCacheResetService.ResetAsync();
        if (!OperatingSystem.IsBrowser())
        {
            ApplicationConfigurationChangedService.NotifyChanged();
        }
    }

    [JSInvokable]
    public virtual async Task OnIdleCallbackAsync(IdleTrackerState state)
    {
        if (state.Idle)
        {
            CountDown = 60;
            SignOutText = L["IdleSignOutNow"] + $" ({CountDown})";
            await ModalRef.Show();
            await IdleTrackerService.PauseAsync();
            SignOutTimer.Period = 1000;
            SignOutTimer.Elapsed += OnSignOutTimerElapsedAsync;
            SignOutTimer.Start();
            await InvokeAsync(StateHasChanged);
        }
    }
    
    [JSInvokable]
    public virtual async Task OnStorageCallbackAsync(IdleTrackerState state)
    {
        if (!state.Idle)
        {
            await ResetIdleTrackerAsync();
        }
    }
    
    protected virtual async Task CloseModalAsync()
    {
        await ResetIdleTrackerAsync(); 
    }

    protected virtual async Task OnSignOutTimerElapsedAsync(AbpAsyncTimer args)
    {
        CountDown--;
        SignOutText = L["IdleSignOutNow"] + $" ({CountDown})";
        if (CountDown <= 0)
        {
            _ = OnSignOutClickedAsync();
            return;
        }

        await InvokeAsync(StateHasChanged);
    }
    
    protected virtual async Task OnSignOutClickedAsync()
    {
        await ResetIdleTrackerAsync();
        await InvokeAsync( async () =>
        {
            await IdleTrackerService.EndAsync();
            NavigateToLogout();
        });
    }
    
    protected virtual void NavigateToLogout()
    {
        NavigationManager.NavigateTo(AuthenticationOptions.Value.LogoutUrl, true);
    }
    
    protected virtual async Task OnStaySignedInClickedAsync()
    {
        await ResetIdleTrackerAsync();
    }
    
    protected virtual async Task ResetIdleTrackerAsync()
    {
        await InvokeAsync(async () =>
        {
            try
            {
                if (ModalRef != null)
                {
                    await ModalRef.Hide();
                }

                await IdleTrackerService.ResumeAsync();
                await IdleTrackerService.ResetTimer();
            }
            catch (Exception)
            {
                //ignore
            }
            finally
            {
                ClearSignOutTimer();
            }
        });
    }
    
    protected virtual Task ClosingModal(ModalClosingEventArgs eventArgs)
    {
        eventArgs.Cancel = eventArgs.CloseReason != CloseReason.UserClosing;
        return Task.CompletedTask;
    }

    public virtual async ValueTask DisposeAsync()
    {
        await ResetIdleTrackerAsync();
        if (IdleTrackerService != null)
        {
            await IdleTrackerService.DisposeAsync();
        }
        DotNetObjectRef?.Dispose();
        ApplicationConfigurationChangedService.Changed -= ApplicationConfigurationChanged;
    }
    
    private void ClearSignOutTimer()
    {
        SignOutTimer.Stop();
        SignOutTimer.Elapsed -= OnSignOutTimerElapsedAsync;
    }
}