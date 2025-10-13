using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AbpSolution1.Maui.Messages;
public class LogoutMessage : ValueChangedMessage<bool?>
{
    public LogoutMessage(bool? value = null) : base(value)
    {
    }
}