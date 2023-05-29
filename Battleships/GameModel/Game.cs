using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.GameModel
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
        private readonly BoardPainter boardPainter;
        private readonly Board board;
        private readonly List<Ship> Ships;

        internal Game(Board board, IEnumerable<Ship> ships, BoardPainter boardPainter)
        {
            this.board = board;
            Ships = ships.ToList();
            this.boardPainter = boardPainter;
        }


        public Tuple<ShotResult, Ship?> ProcessShot(string xDescr, string yDescr)
        {
            var (x, y) = board.ConvertToCoordinates(xDescr, yDescr);
            var shipComponent = board.ProcessShot(x, y);

            if (shipComponent == null)
            {
                boardPainter.PaintMissDot(x, y);
                return Tuple.Create<ShotResult, Ship?>(ShotResult.Miss, null);
            }

            boardPainter.PaintShipComponent(x, y, ShipComponentState.Hit);
            ShotResult shotResult = ShotResult.Hit; 

            if (shipComponent.Ship.IsSunk)
            {
                boardPainter.PaintSunkShip(shipComponent.Ship);
                shotResult |= ShotResult.ShipSunk;
            }

            if (AllShipsAreSunk())
            {
                shotResult |= ShotResult.GameEnd;
            }
            return new Tuple<ShotResult, Ship?>(shotResult, shipComponent.Ship);
        }


        private bool AllShipsAreSunk()
        {
            return Ships.All(ship => ship.IsSunk);
        }

        public void Paint(bool debugMode)
        {
            boardPainter.PaintAll(board, debugMode);
        }
        public void Clear()
        {
            boardPainter.Clear();
        }
    }
}
