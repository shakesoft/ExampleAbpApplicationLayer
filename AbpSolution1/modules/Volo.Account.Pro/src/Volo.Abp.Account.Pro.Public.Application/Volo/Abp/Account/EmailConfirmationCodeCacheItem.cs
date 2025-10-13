using System;
using System.Collections.Generic;

namespace Volo.Abp.Account;

[Serializable]
public class EmailConfirmationCodeCacheItem
{
    public string EmailAddress { get; set; }

    public string Code { get; set; }

    public bool Valid { get; set; }

    public DateTime SendTime { get; set; }

    public int SendCount { get; set; }

    public List<DateTime> SendRecords { get; set; }

    public int TryCount { get; set; }

    public DateTime? LastTryTime { get; set; }

    public DateTime? NextTryTime { get; set; }

    public DateTime? NextSendTime { get; set; }

    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
}
