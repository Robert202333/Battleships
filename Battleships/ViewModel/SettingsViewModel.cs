using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GameModel;

namespace Battleships
{
    
    internal class SettingsViewModel
    {
        public Settings? Settings { get; set; }

        public List<CoordinateDescriptionType> CoordinateDescriptionTypes { get; } 
            = Enum.GetValues(typeof(CoordinateDescriptionType)).Cast<CoordinateDescriptionType>().ToList();

        public SettingsViewModel()
        {

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
