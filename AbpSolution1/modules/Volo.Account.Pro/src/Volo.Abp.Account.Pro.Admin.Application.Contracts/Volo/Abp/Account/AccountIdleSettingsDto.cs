namespace Volo.Abp.Account;

public class AccountIdleSettingsDto
{
    public bool Enabled { get; set; }

    public int IdleTimeoutMinutes { get; set; }
}
