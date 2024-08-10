using CommunityToolkit.Mvvm.Messaging.Messages;
using Expandroid.Models;

public class AcServiceMessage : ValueChangedMessage<(string, Match)>
{
    public AcServiceMessage((string, Match) message) : base(message)
    {
    }
}