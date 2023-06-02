// See https://aka.ms/new-console-template for more information
using GameModel;


var game = new ConsoleGame();
game.Run();

internal class ConsoleGame
{
    const string initMessage = "Battleships";
    const string newGameMessage = "New game";
    const string exitMessage = "Thank you for playing";
    const string invalidCoordinatesFormat = "Invalid coordinates format";

    const string exitCmd = "EXIT";
    const string newCmd = "NEW";

    private GameEnv gameEnv = new GameEnv(new DefaultGameCreator(), new MessageDisplayer());

    private bool exit = false;


    readonly private Dictionary<string, Action> commandMap = new();
    internal ConsoleGame()
    {
        commandMap.Add(exitCmd, OnExit);
        commandMap.Add(newCmd, OnNew);

        gameEnv.Painter = new BoardPainter();
        Console.WriteLine(initMessage);
        OnNew();
    }
    internal void Run()
    {
        while(!exit)
        {   
            string input = GetInput();
            string upperInput = input.ToUpper();

            if (commandMap.TryGetValue(upperInput, out Action? action))
                action();
            else
                OnCoordinates(upperInput);
        }
        Console.WriteLine(exitMessage);
    }

    private string GetInput()
    {
        Console.WriteLine();
        Console.Write(CreatePrompt());
        return Console.ReadLine() ?? "";
    }
    private string CreatePrompt()
    {
        return $"Exit/New{(gameEnv.GameActive ? "/Shot coordinates" : "")} >> ";
    }

    private void OnExit()
    {
        exit = true;
    }

    private void OnNew()
    {
        Console.WriteLine();
        Console.WriteLine(newGameMessage);
        gameEnv.Restart();
    }

    private void OnCoordinates(string input)
    {
        try
        {
            var (xCoor, yCoor) = input.ConvertToCoordinates();
            if (gameEnv.ProcessShot(xCoor, yCoor))
                gameEnv.Restart();
        }
        catch 
        {
            Console.WriteLine(invalidCoordinatesFormat);
        }
    }
}


