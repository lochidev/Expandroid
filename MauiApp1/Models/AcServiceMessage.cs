using CommunityToolkit.Mvvm.Messaging.Messages;

public class AcServiceMessage : ValueChangedMessage<(string, KeyValuePair<string, string>)>
{
    public AcServiceMessage((string, KeyValuePair<string, string>) message) : base(message)
    {
    }
}