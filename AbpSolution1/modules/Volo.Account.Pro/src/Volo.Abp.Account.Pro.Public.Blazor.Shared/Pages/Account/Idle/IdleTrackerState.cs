namespace Volo.Abp.Account.Pro.Public.Blazor.Shared.Pages.Account.Idle;

public class IdleTrackerState
{
    public bool Idle { get; set; }
    
    public bool Paused { get; set; }
    
    public long LastActive { get; set; }
}