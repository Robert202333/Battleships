using GameModel;

internal class MessageDisplayer : IMessageDisplayer
{
    public void ShowWarning(string type, string message)
    {
        Console.WriteLine($"{type}: {message} (warning)");
    }
    public void ShowError(string type, string message)
    {
        Console.WriteLine($"{type}: {message} (error)");
    }
    public void ShowInformation(string type, string message)
    {
        Console.WriteLine($"{type}: {message} (info)");
    }
}


