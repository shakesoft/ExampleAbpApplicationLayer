using CommunityToolkit.Mvvm.Messaging.Messages;
using Volo.Abp.Identity;
using Volo.Saas.Host.Dtos;

namespace AbpSolution1.Maui.Messages;
public class IdentityUserEditMessage : ValueChangedMessage<IdentityUserEditMessageArgs>
{
    public IdentityUserEditMessage(IdentityUserEditMessageArgs value) : base(value)
    {
    }
}

public class IdentityUserEditMessageArgs
{
    public Guid UserId { get; set; }

    public IdentityUserUpdateDto? User { get; set; }
}
