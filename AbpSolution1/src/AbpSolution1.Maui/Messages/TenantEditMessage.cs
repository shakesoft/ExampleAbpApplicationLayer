using CommunityToolkit.Mvvm.Messaging.Messages;
using Volo.Saas.Host.Dtos;

namespace AbpSolution1.Maui.Messages;

public class TenantEditMessage : ValueChangedMessage<SaasTenantUpdateDto>
{
    public TenantEditMessage(SaasTenantUpdateDto value) : base(value)
    {
    }
}
