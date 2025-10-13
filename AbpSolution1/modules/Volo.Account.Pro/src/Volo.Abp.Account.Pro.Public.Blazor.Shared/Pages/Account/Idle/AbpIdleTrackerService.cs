using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Account.Pro.Public.Blazor.Shared.Pages.Account.Idle;

public class AbpIdleTrackerService<T> : IScopedDependency, IAsyncDisposable where T : class
{
    protected IJSRuntime JsRuntime { get; }

    protected IJSObjectReference IdleTrackerRef { get; set; }
    

    public AbpIdleTrackerService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }
    
    public virtual async Task InitializeAsync(DotNetObjectReference<T> dotNetObjectRef, string onAccountIdleSettingsChangedCallbackMethod)
    {
        await JsRuntime.InvokeVoidAsync("abp.idleTracker.initialize", new
        {
            dotNetObjectRef,
            onAccountIdleSettingsChangedCallbackMethod
        });
    }

    public virtual async Task CreateAsync(int timeout, string onIdleCallbackMethod, string onStorageCallbackMethod)
    {
        IdleTrackerRef = await JsRuntime.InvokeAsync<IJSObjectReference>(
            "abp.idleTracker.create", new {
                timeout,
                onIdleCallbackMethod,
                onStorageCallbackMethod
            });
    }

    public virtual async Task StartAsync()
    {
        CheckInstance();

        await IdleTrackerRef.InvokeVoidAsync("start");
    }

    public virtual async Task EndAsync()
    {
        CheckInstance();

        await IdleTrackerRef.InvokeVoidAsync("end");
    }

    public virtual async Task SyncStateAsync()
    {
        CheckInstance();

        await IdleTrackerRef.InvokeVoidAsync("syncState");
    }

    public virtual async Task ResumeAsync()
    {
        CheckInstance();

        await IdleTrackerRef.InvokeVoidAsync("resume");
    }
    
    public virtual async Task PauseAsync()
    {
        CheckInstance();

        await IdleTrackerRef.InvokeVoidAsync("pause");
    }

    public virtual async Task ResetTimer()
    {
        CheckInstance();

        await IdleTrackerRef.InvokeVoidAsync("resetTimer");
    }

    protected virtual void CheckInstance()
    {
        if (IdleTrackerRef == null)
        {
            throw new InvalidOperationException("AbpIdleTrackerService is not initialized!");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (IdleTrackerRef != null)
        {
            try
            {
                await PauseAsync();
                await EndAsync();
                await IdleTrackerRef.DisposeAsync();
            }
            catch (Exception e)
            {
                if(e is JSDisconnectedException or ObjectDisposedException)
                {
                    //ignore
                }
                else
                {
                    throw;
                }
            }
        }

        IdleTrackerRef = null;
    }
}