using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{
    public enum ShotResult
    {
        Miss = 1,
        Hit = 2,
        ShipSunk = 4,
        GameEnd = 8
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
                throw new RepeatedHitException();

            square.WasHit = true;
            if (square.ShipComponent != null)
                square.ShipComponent.WasHit = true;

            if (square.ShipComponent == null)
                return Tuple.Create<Square, ShotResult>(square, ShotResult.Miss);

            ShotResult shotResult = ShotResult.Hit;

            if (square.ShipComponent.Ship.WassSunk)
                shotResult |= ShotResult.ShipSunk;

            if (AllShipsWereSunk())
                shotResult |= ShotResult.GameEnd;

            return new Tuple<Square, ShotResult>(square, shotResult);
        }


        private bool AllShipsWereSunk()
        {
            return Ships.All(ship => ship.WassSunk);
        }
    }
}
