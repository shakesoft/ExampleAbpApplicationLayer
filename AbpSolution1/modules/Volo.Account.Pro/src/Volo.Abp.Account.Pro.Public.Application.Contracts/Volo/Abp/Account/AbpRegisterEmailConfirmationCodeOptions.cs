using System;

namespace Volo.Abp.Account;

public class AbpRegisterEmailConfirmationCodeOptions
{
    public int DailySendLimit { get; set; }

    public int HourlySendLimit { get; set; }

    public TimeSpan CodeExpirationTime { get; set; }

    public int MaxFailedCheckCount { get; set; }
}
