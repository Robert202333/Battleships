// See https://aka.ms/new-console-template for more information
using GameModel;


var game = new ConsoleGame();
game.Execute();

internal class ConsoleGame
{
    const string initMessage = "Battleships";
    const string newGameMessage = "New game";
    const string exitMessage = "Thank you for playing";
    const string invalidCoordinatesFormat = "Invalid coordinate format";

    const string exitCmd = "EXIT";
    const string newCmd = "NEW";

    private GameEnv gameEnv = new GameEnv(new DefaultGameCreator(), new MessageDisplayer());

    private bool exit = false;

    internal ConsoleGame()
    {
        gameEnv.Painter = new BoardPainter();
        Console.WriteLine(initMessage);
        OnNew();
    }
    internal void Execute()
    {
        while(!exit)
        {
            Console.WriteLine();
            Console.Write(CreatePrompt());
            string? read = Console.ReadLine();
            string input = read != null ? read.ToUpper().Trim() : "";

            switch (input)
            {
                case exitCmd:
                    OnExit();
                    break;

                case newCmd:
                    OnNew();
                    break;

                default:
                    OnCoordinates(input);
                    break;
            }
        }
        Console.WriteLine(exitMessage);
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
        Console.WriteLine();
        gameEnv.Restart();
    }

    private void OnCoordinates(string input)
    {
        try
        {
            var (xCoor, yCoor) = gameEnv.ConvertToCoordinates(input);
            if (gameEnv.ProcessShot(xCoor, yCoor))
                gameEnv.Restart();
        }
        catch 
        {
            Console.WriteLine(invalidCoordinatesFormat);
        }
    }
}


