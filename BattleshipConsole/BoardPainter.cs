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

    public void PaintShotResult(Tuple<Square, ShotResult> shotResult, Board board, bool debugMode)
    {
        PaintAll(board, debugMode);
    }

    public void Clear()
    {
        // No action in console app
    }

    public void PaintAll(Board board, bool debugMode)
    {
        PaintHorizontalDescriptions(board);
        for (uint i = 0; i < board.VerticalDescriptor.Size; i++)
            PaintHoriontalLine(board, i);
        PaintHorizontalDescriptions(board);
        Console.WriteLine();
    }

    private void PaintHorizontalDescriptions(Board board)
    {
        PaintForamatted("");
        for (uint i = 0; i < board.HorizontalDescriptor.Size; i++)
            PaintForamatted(board.HorizontalDescriptor.GetDescription(i));
        Console.WriteLine();
    }

    private void PaintHoriontalLine(Board board, uint verticalIndex)
    {
        PaintForamatted(board.VerticalDescriptor.GetDescription(verticalIndex));
        for (uint i = 0; i < board.HorizontalDescriptor.Size; i++)
            PaintSquare(board.GetSquare(i, verticalIndex));
        Console.WriteLine(board.VerticalDescriptor.GetDescription(verticalIndex));
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
