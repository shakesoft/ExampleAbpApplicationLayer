using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AbpSolution1.Maui.Messages;

public class LanguageChangedMessage : ValueChangedMessage<string?>
{
    public LanguageChangedMessage(string? value) : base(value)
    {
    }
}
