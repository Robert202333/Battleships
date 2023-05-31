using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{
    public class GameEnv
    {
        private readonly IMessageDisplayer showGameMessage;
        private readonly AbstractGameCreator gameCreator;
        private Game? game;
        public Settings Settings {get;} = new Settings();

        public IBoardPainter? Painter { get; set; }

        public bool GameActive { get { return game != null; } }

        public GameEnv(AbstractGameCreator gameCreator, IMessageDisplayer showGameMessage) 
        {
            this.showGameMessage = showGameMessage;
            this.gameCreator = gameCreator;
        }
                
        public bool Restart()
        {
            Painter?.Clear();
            game = null;
            try
            {
                game = gameCreator.Execute(Settings);
                Painter?.PaintAll(game, Settings.DebugMode);
            }
            catch (ShipCreationException e)
            {
                showGameMessage.ShowWarning("Game creation", e.Message);
            }
            catch (Exception e)
            {
                showGameMessage.ShowError("Game creation", e.Message);
            }
            return game != null;
        }

        // Returns true if game was finished
        public bool ProcessShot(string xCoor, string yCoor)
        {
            try
            {
                Tuple<Square, ShotResult> result = game!.ProcessShot(xCoor, yCoor);
                Painter?.PaintShotResult(result, game, Settings.DebugMode);

                var (square, shotResult) = result;

                if ((shotResult & ShotResult.GameEnd) != 0)
                {
                    showGameMessage.ShowInformation("Shot result", $"{square.ShipComponent!.Ship.Name} was sunk and you won !");
                }
                else if ((shotResult & ShotResult.ShipSunk) != 0)
                {
                    showGameMessage.ShowInformation("Shot result", $"{square.ShipComponent!.Ship.Name} was sunk !");
                }
                else if ((shotResult & ShotResult.Hit) != 0)
                {
                    showGameMessage.ShowInformation("Shot result", $"{square.ShipComponent!.Ship.Name} was hit !");
                }
                return (shotResult & ShotResult.GameEnd) != 0;
            }
            catch (Exception e)
            {
                showGameMessage.ShowWarning("Shot error", e.Message);
                return false;
            }
        }

        public Tuple<string, string> ConvertToCoordinates(string coordinates)
        {
            string upperCoordinates = coordinates;

            string xCoor = "";
            string yCoor = "";

            upperCoordinates.Trim();
            if (upperCoordinates.Length < 2)
                throw new InvalidDescriptionException();

            if (char.IsLetter(upperCoordinates[0]))
            {
                xCoor += upperCoordinates[0];
                if (char.IsDigit(upperCoordinates[1]))
                    yCoor = ReadNumber(upperCoordinates, 1);
            }
            else if (char.IsDigit(upperCoordinates[0]))
            {
                xCoor = ReadNumber(upperCoordinates, 0);
                if (upperCoordinates.Length > xCoor.Length && char.IsLetter(upperCoordinates[xCoor.Length]))
                {
                    yCoor += upperCoordinates[xCoor.Length];
                }
            }
            if (xCoor.Length > 0 && yCoor.Length > 0)
                return Tuple.Create(xCoor, yCoor);
            else
                throw new InvalidDescriptionException();


            // Reads up to 2 digit number
            static string ReadNumber(string coor, int startIndex)
            {
                string result = "";
                for (int i = startIndex; i <= startIndex + 1 && i < coor.Length && char.IsDigit(coor[i]); i++)
                    result += coor[i];
                return result;
            }
        }

        public void Paint()
        {
            if (game != null)
                Painter?.PaintAll(game, Settings.DebugMode);
        }

        public void OnSettingsChange()
        {
            Settings.Validate();
            Painter?.OnSettingsChange(Settings.HorizontalSize, Settings.VerticalSize);
        }
    }
}
