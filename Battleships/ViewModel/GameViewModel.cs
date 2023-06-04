using Battleships.Painter;
using GameModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;


namespace Battleships
{
    public class GameViewModel : INotifyPropertyChanged
    {

        // Commands
        public Command StartNewGameCmd { get; init; }
        public Command SettingsCmd { get; init; }
        public Command ShotCmd { get; init; }



        private GameEnv gameEnv;


        public string shotCoordinates = "";
        public string ShotCoordinates
        {
            get { return shotCoordinates; }
            set
            {
                shotCoordinates = value;
                ShotCmd.InvokeCanExecuteChanged();
            }
        }

        private bool gameActive = false;
        public bool GameActive
        {
            get { return gameActive; }
            set
            {
                gameActive = value;
                OnPropertyChanged();
                ShotCmd.InvokeCanExecuteChanged();
            }
        }


        public GameViewModel()
        {
            // Initialize commands
            StartNewGameCmd = new Command((object? obj) => StartNewGame());
            SettingsCmd = new Command((object? obj) => Settings());
            ShotCmd = new Command((object? obj) => Shot(), (object? obj) => GameActive && ShotCcordinatesHaveValidFormat());

            gameEnv = new GameEnv(new DefaultGameCreator(), new MessageDisplayer());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Canvas (set by main window)
        public void SetCanvas(Canvas canvas)
        {
            gameEnv.Painter = new BoardPainter(canvas);
            gameEnv.OnSettingsChange();
        }

        // Called by main window
        public void OnCanvasResize()
        {
            (gameEnv.Painter as BoardPainter)!.OnCanvasResize();
            gameEnv.Paint();
        }

        private bool ShotCcordinatesHaveValidFormat()
        {
            return ShotCoordinates.ConvertToCoordinates() != null;
        }

        private void Settings()
        {
            SettingsDialog settingsDialog = new SettingsDialog(gameEnv.Settings);
            settingsDialog.ShowDialog();

            gameEnv.OnSettingsChange();

            StartNewGame();
        }

        private void Shot()
        {
            var (xCoor, yCoor) = ShotCoordinates.ToUpper().ConvertToCoordinates()!;
            if (gameEnv.ProcessShot(xCoor, yCoor))
                StartNewGame();
        }

        public void StartNewGame()
        {
            GameActive = gameEnv.Restart();
        }

    }
}
