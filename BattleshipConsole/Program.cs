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

    internal ConsoleGame()
    {
        gameEnv.Painter = new BoardPainter();
        Console.WriteLine(initMessage);
        OnNew();
    }
    internal void Run()
    {
        while(!exit)
        {
            Console.WriteLine();
            Console.Write(CreatePrompt());
            string? input = Console.ReadLine();
            string upperInput = input != null ? input.ToUpper().Trim() : "";

            switch (upperInput)
            {
                case exitCmd:
                    OnExit();
                    break;

                case newCmd:
                    OnNew();
                    break;

                default:
                    OnCoordinates(upperInput);
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


