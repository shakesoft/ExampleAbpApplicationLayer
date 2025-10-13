using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;

namespace Volo.Abp.Account;

public class SendEmailConfirmationCodeDto
{
    [Required]
    public string EmailAddress { get; set; }

    [Required]
    public string UserName { get; set; }

    [DisableAuditing]
    public string CaptchaResponse { get; set; }
}
