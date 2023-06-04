namespace GameModel
{
    [Flags]
    public enum ShotResult
    {
        Miss = 1,
        Hit = 2,
        ShipSunk = 4,
        GameEnd = 8,
        Repeated = 16
    }

    internal class Game
    {
        internal Board Board { get; init; }

        private readonly List<Ship> Ships;

        internal Game(Board board, IEnumerable<Ship> ships)
        {
            Board = board;
            Ships = ships.ToList();
        }



        internal Tuple<Square, ShotResult> ProcessShot(string xDescr, string yDescr)
        {
            var coordinates = Board.ConvertToCoordinates(xDescr, yDescr);
            var square = Board.GetSquare(coordinates.X, coordinates.Y);

            if (square.WasHit)
                return Tuple.Create(square, ShotResult.Repeated);

            square.WasHit = true;

            if (square.ShipComponent == null)
                return Tuple.Create(square, ShotResult.Miss);

            square.ShipComponent.WasHit = true;
            ShotResult shotResult = ShotResult.Hit;

            if (square.ShipComponent.Ship.WasSunk)
                shotResult |= ShotResult.ShipSunk;

            if (AllShipsWereSunk())
                shotResult |= ShotResult.GameEnd;

            return Tuple.Create(square, shotResult);
        }


        private bool AllShipsWereSunk()
        {
            return Ships.All(ship => ship.WasSunk);
        }
    }
}
