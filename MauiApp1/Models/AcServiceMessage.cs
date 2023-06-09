using CommunityToolkit.Mvvm.Messaging.Messages;
using MauiApp1.Models;

public class AcServiceMessage : ValueChangedMessage<(string, Match)>
{
    public AcServiceMessage((string, Match) message) : base(message)
    {
    }
}