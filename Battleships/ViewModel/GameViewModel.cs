using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Battleships.GameModel;

namespace Battleships
{
    public class GameViewModel : INotifyPropertyChanged
    {

        // Commands
        public Command StartNewGameCmd { get; init; }
        public Command SettingsCmd { get; init; }
        public Command ShotCmd { get; init; }


        BoardPainter boardPainter;

        // Canvas (set by main window)
        public void SetCanvas(Canvas canvas)
        {
            boardPainter = new BoardPainter(canvas);
            boardPainter.OnSettingsChange(settings.HorizontalSize, settings.VerticalSize);
        }

        // Called by main window
        public void OnCanvasResize()
        {
            boardPainter?.OnCanvasResize();
            game?.Paint(settings.DebugMode);
        }


        private Settings settings;
        private Game? game = null;

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


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.        
// warning disable is for boardPainter onject which is not created in constructor. 
        public GameViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            settings = new Settings();

            // Initialize commands
            StartNewGameCmd = new Command((object? obj) => StartNewGame());
            SettingsCmd = new Command((object? obj) => Settings());
            ShotCmd = new Command((object? obj) => Shot(), (object? obj) => GameActive && ShotCcordinatesHaveValidFormat());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool ShotCcordinatesHaveValidFormat()
        {
            bool shotCoordinatesAreValid = true;
            try
            {
                ReadShotCoordinates();
            }
            catch
            {
                shotCoordinatesAreValid = false;
            }
            return shotCoordinatesAreValid;
        }

        private void StartNewGame()
        {
            StopGame();
            try
            {
                SetNewGame(GameCreator.Create(settings, boardPainter));
            }
            catch (ShipCreationException e)
            {
                MessageBox.Show(e.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void Settings()
        {
            SettingsDialog settingsDialog = new SettingsDialog(settings);
            settingsDialog.ShowDialog();
            settings.Validate();

            StopGame();

            boardPainter.OnSettingsChange(settings.HorizontalSize, settings.VerticalSize);
        }

        private void Shot()
        {
            var (xCoor, yCoor) = ReadShotCoordinates();

            try
            {
                Tuple<ShotResult, Ship?> result = game!.ProcessShot(xCoor, yCoor);

                if ((result.Item1 & ShotResult.GameEnd) != 0)
                {
                    MessageBox.Show($"{result.Item2!.Name} was sunk and you won !", "Shot result", MessageBoxButton.OK, MessageBoxImage.Information);
                    StopGame();
                }
                else if ((result.Item1 & ShotResult.ShipSunk) != 0)
                {
                    MessageBox.Show($"{result.Item2!.Name} was sunk !", "Shot result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if ((result.Item1 & ShotResult.Hit) != 0)
                {
                    MessageBox.Show($"{result.Item2!.Name} was hit !", "Shot result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } 
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Shot error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private Tuple<string, string> ReadShotCoordinates()
        {
            string upperCoor = ShotCoordinates.ToUpper();

            string xCoor = "";
            string yCoor = "";

            upperCoor.Trim();
            if (upperCoor.Length < 2)
                throw new InvalidDescriptionException();

            if (char.IsLetter(upperCoor[0]))
            {
                xCoor += upperCoor[0];
                if (char.IsDigit(upperCoor[1]))
                    yCoor = ReadNumber(upperCoor, 1);
            }
            else if (char.IsDigit(upperCoor[0]))
            {
                xCoor = ReadNumber(upperCoor, 0);
                if (upperCoor.Length > xCoor.Length && char.IsLetter(upperCoor[xCoor.Length]))
                {
                    yCoor += upperCoor[xCoor.Length];
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
                for(int i = startIndex; i <= startIndex + 1 && i < coor.Length && char.IsDigit(coor[i]); i++)
                    result += coor[i];
                return result;
            }
        }

        private void SetNewGame(Game game)
        {
            this.game = game;
            this.game.Paint(settings.DebugMode);
            GameActive = true;
        }

        private void StopGame()
        {
            this.game?.Clear();
            this.game = null;
            GameActive = false;
        }

    }
}
