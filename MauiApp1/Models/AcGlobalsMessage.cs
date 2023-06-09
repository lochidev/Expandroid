using CommunityToolkit.Mvvm.Messaging.Messages;
using MauiApp1.Models;

public class AcGlobalsMessage : ValueChangedMessage<List<Var>>
{
    public AcGlobalsMessage(List<Var> message) : base(message)
    {
    }
}