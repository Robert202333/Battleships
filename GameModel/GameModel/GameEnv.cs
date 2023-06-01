using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{
    internal static class MessageType
    {
        internal const string GameCreation = "Game creation";
        internal const string ShotResult = "Shot result";
        internal const string ShotError = "Shot error";
    }

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
                Painter?.PaintAll(game.Board, Settings.DebugMode);
            }
            catch (ShipCreationException e)
            {
                showGameMessage.ShowWarning(MessageType.GameCreation, e.Message);
            }
            catch (Exception e)
            {
                showGameMessage.ShowError(MessageType.GameCreation, e.Message);
            }
            return game != null;
        }

        // Returns true if game was finished
        public bool ProcessShot(string xCoor, string yCoor)
        {
            try
            {
                Tuple<Square, ShotResult> result = game!.ProcessShot(xCoor, yCoor);
                Painter?.PaintShotResult(result, game.Board, Settings.DebugMode);

                var (square, shotResult) = result;

                if ((shotResult & ShotResult.GameEnd) != 0)
                {
                    showGameMessage.ShowInformation(MessageType.ShotResult, $"{square.ShipComponent!.Ship.Name} was sunk and you won !");
                }
                else if ((shotResult & ShotResult.ShipSunk) != 0)
                {
                    showGameMessage.ShowInformation(MessageType.ShotResult, $"{square.ShipComponent!.Ship.Name} was sunk !");
                }
                else if ((shotResult & ShotResult.Hit) != 0)
                {
                    showGameMessage.ShowInformation(MessageType.ShotResult, $"{square.ShipComponent!.Ship.Name} was hit !");
                }
                return (shotResult & ShotResult.GameEnd) != 0;
            }
            catch (Exception e)
            {
                showGameMessage.ShowWarning(MessageType.ShotError, e.Message);
                return false;
            }
        }

        // Returns when coordinates have valid format, despite are valid in board context or not
        public Tuple<string, string> ConvertToCoordinates(string coordinates)
        {
            string xCoor = "";
            string yCoor = "";

            coordinates.Trim();
            if (coordinates.Length < 2 || coordinates.Length > 3)
                throw new InvalidDescriptionException();

            if (char.IsLetter(coordinates[0]))
            {
                xCoor += coordinates[0];
                if (char.IsDigit(coordinates[1]))
                    yCoor = ReadNumber(coordinates, 1);
            }
            else if (char.IsDigit(coordinates[0]))
            {
                xCoor = ReadNumber(coordinates, 0);
                if (coordinates.Length > xCoor.Length && char.IsLetter(coordinates[xCoor.Length]))
                {
                    yCoor += coordinates[xCoor.Length];
                }
            }
            if (xCoor.Length > 0 && yCoor.Length > 0 && xCoor.Length + yCoor.Length == coordinates.Length)
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
                Painter?.PaintAll(game.Board, Settings.DebugMode);
        }

        public void OnSettingsChange()
        {
            Settings.Validate();
            Painter?.OnSettingsChange(Settings.HorizontalSize, Settings.VerticalSize);
        }
    }
}
