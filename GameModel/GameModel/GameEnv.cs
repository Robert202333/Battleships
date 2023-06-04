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
        public Settings Settings { get; } = new Settings();

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
                var (square, shotResult) = result;
                if (shotResult.HasFlag(ShotResult.Repeated))
                {
                    showGameMessage.ShowWarning(MessageType.ShotError, "The same square hit again");
                    return false;
                }

                Painter?.PaintShotResult(result, game.Board, Settings.DebugMode);

                if (shotResult.HasFlag(ShotResult.GameEnd))
                {
                    showGameMessage.ShowInformation(MessageType.ShotResult, $"{square.ShipComponent!.Ship.Name} was sunk and you won !");
                }
                else if (shotResult.HasFlag(ShotResult.ShipSunk))
                {
                    showGameMessage.ShowInformation(MessageType.ShotResult, $"{square.ShipComponent!.Ship.Name} was sunk !");
                }
                else if (shotResult.HasFlag(ShotResult.Hit))
                {
                    showGameMessage.ShowInformation(MessageType.ShotResult, $"{square.ShipComponent!.Ship.Name} was hit !");
                }
                return shotResult.HasFlag(ShotResult.GameEnd);
            }
            catch (Exception e)
            {
                showGameMessage.ShowWarning(MessageType.ShotError, e.Message);
                return false;
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
