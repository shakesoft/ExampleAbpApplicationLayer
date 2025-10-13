using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AbpSolution1.Maui.Messages;

public class ProfilePictureChangedMessage : ValueChangedMessage<string>
{
    public ProfilePictureChangedMessage(string value) : base(value)
    {
    }
}
