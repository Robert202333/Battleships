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

    public class Game
    {
        public Board Board { get; init; }

        private readonly List<Ship> Ships;

        internal Game(Board board, IEnumerable<Ship> ships)
        {
            Board = board;
            Ships = ships.ToList();
        }



        internal Tuple<Coordinates, ShotResult, ShipComponent?> ProcessShot(string xDescr, string yDescr)
        {
            var coordinates = Board.ConvertToCoordinates(xDescr, yDescr);
            var shipComponent = Board.ProcessShot(coordinates.X, coordinates.Y);

            if (shipComponent == null)
                return Tuple.Create<Coordinates, ShotResult, ShipComponent?>(coordinates, ShotResult.Miss, null);

            ShotResult shotResult = ShotResult.Hit;

            if (shipComponent.Ship.WassSunk)
                shotResult |= ShotResult.ShipSunk;

            if (AllShipsWereSunk())
                shotResult |= ShotResult.GameEnd;

            return new Tuple<Coordinates, ShotResult, ShipComponent?>(coordinates, shotResult, shipComponent);
        }


        private bool AllShipsWereSunk()
        {
            return Ships.All(ship => ship.WassSunk);
        }
    }
}
