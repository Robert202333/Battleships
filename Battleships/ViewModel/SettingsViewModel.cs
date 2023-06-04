using GameModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Battleships
{

    internal class SettingsViewModel : INotifyPropertyChanged
    {
        private Settings? settings;
        public Settings? Settings
        {
            get { return settings; }
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public List<CoordinateDescriptionType> CoordinateDescriptionTypes { get; }
            = Enum.GetValues(typeof(CoordinateDescriptionType)).Cast<CoordinateDescriptionType>().ToList();

        public SettingsViewModel()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CoordinateDescriptionTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return (CoordinateDescriptionType)value;
        }
    }

}
