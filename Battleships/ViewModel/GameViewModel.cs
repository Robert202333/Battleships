using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using GameModel;
using Battleships.Painter;


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

            gameEnv = new GameEnv(new DefaultGameCreator(), new MessageDisplayers());
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
            bool shotCoordinatesAreValid = true;
            try
            {
                gameEnv.ConvertToCoordinates(ShotCoordinates);
            }
            catch
            {
                shotCoordinatesAreValid = false;
            }
            return shotCoordinatesAreValid;
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
            var (xCoor, yCoor) = gameEnv.ConvertToCoordinates(ShotCoordinates.ToUpper());
            if (gameEnv.ProcessShot(xCoor, yCoor))
                StartNewGame();
        }

        public void StartNewGame()
        {
            GameActive = gameEnv.Restart();
        }

    }
}
