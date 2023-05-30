using GameModel;

public class BoardPainter : IBoardPainter
{
    const char emptySquare = ' ';
    const char missedShot = '-';
    const char componentHit = '+';
    const char componentSunk = 'X';

    public BoardPainter()
    {

    }

    public void OnSettingsChange(uint horizontalSize, uint verticalSize)
    {
        // settings change not provide for console
    }

    public void PaintShotResult(Tuple<Coordinates, ShotResult, ShipComponent?> shotResult, Game game, bool debugMode)
    {
        PaintAll(game, debugMode);
    }

    public void Clear()
    {
        // No action in console app
    }

    public void PaintAll(Game game, bool debugMode)
    {
        PaintHorizontalDescriptions(game);
        for (uint i = 0; i < game.Board.VerticalDescriptor.Size; i++)
            PaintHoriontalLine(game, i);
        PaintHorizontalDescriptions(game);
        Console.WriteLine();
    }

    private void PaintHorizontalDescriptions(Game game)
    {
        PaintForamatted("");
        for (uint i = 0; i < game.Board.HorizontalDescriptor.Size; i++)
            PaintForamatted(game.Board.HorizontalDescriptor.GetDescription(i));
        Console.WriteLine();
    }

    private void PaintHoriontalLine(Game game, uint verticalIndex)
    {
        PaintForamatted(game.Board.VerticalDescriptor.GetDescription(verticalIndex));
        for (uint i = 0; i < game.Board.HorizontalDescriptor.Size; i++)
            PaintSquare(game.Board.GetSquare(i, verticalIndex));
        Console.WriteLine(game.Board.VerticalDescriptor.GetDescription(verticalIndex));
    }

    private void PaintSquare(Square square)
    {
        var mark = square.ShipComponent == null ?
            (square.WasHit ? missedShot : emptySquare) :
            (square.ShipComponent.WasHit ? 
                square.ShipComponent.Ship.WassSunk ? componentSunk : componentHit :
                emptySquare);

        PaintForametted(mark);
    }

    private void PaintForamatted(string txt)
    {
        Console.Write("{0,-2 }", txt);
    }

    private void PaintForametted(char txt)
    {
        Console.Write("{0,-2 }", txt);
    }
}
