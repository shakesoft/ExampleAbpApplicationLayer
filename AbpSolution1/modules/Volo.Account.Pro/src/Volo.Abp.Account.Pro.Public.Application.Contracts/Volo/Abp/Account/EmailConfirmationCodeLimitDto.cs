using System;

namespace Volo.Abp.Account;

public class EmailConfirmationCodeLimitDto
{
    public DateTime? NextSendTime { get; set; }

    public DateTime? NextTryTime { get; set; }
}
