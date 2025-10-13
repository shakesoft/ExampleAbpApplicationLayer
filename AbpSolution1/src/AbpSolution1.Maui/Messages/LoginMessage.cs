using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AbpSolution1.Maui.Messages;
public class LoginMessage : ValueChangedMessage<bool?>
{
    public LoginMessage(bool? value = null) : base(value)
    {
    }
}
