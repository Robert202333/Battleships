using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.GameModel
{
    public enum CoordinateDescriptionType
    {
        Number,
        Letter,
    }

    public class ShipDescription
    {
        public string Name { get; set; } = "";
        public uint Size { get; set; }
        public uint Count { get; set; }
    }

    public class Settings
    {
        public uint HorizontalSize { get; set; } = 10;
        public uint VerticalSize { get; set; } = 10;

        public CoordinateDescriptionType HorizontalCoordinateDescriptionType { get; set; } = CoordinateDescriptionType.Letter;
        public CoordinateDescriptionType VerticalCoordinateDescriptionType { get; set; } = CoordinateDescriptionType.Number;

        public List<ShipDescription> ShipDescriptions { get; set; } = new ()
        {
            new () { Name = "Destroyer", Count = 2, Size = 2},
            new () { Name = "Submarine", Count = 1, Size = 3},
            new () { Name = "Cruiser", Count = 1, Size = 4},
            new () { Name = "Carrier", Count = 1, Size = 5}
        };

        public bool StrightShips { get; set; } = true;
        public bool ShipsCanStick { get; set; } = false;

        public bool DebugMode { get; set; } = false;
        public Settings()
        {

        }

        // UI doesn't provude any validation. Called after leaving settings dialog
        public void Validate()
        {
            static uint Clamp(uint value, uint min, uint max)
            {
                return (value < min) ? min : (value > max) ? max : value;
            }

            HorizontalSize = Clamp(HorizontalSize, 5, 25);
            VerticalSize = Clamp(VerticalSize, 5, 25);

            if (VerticalCoordinateDescriptionType == HorizontalCoordinateDescriptionType)
                HorizontalCoordinateDescriptionType = 1 - VerticalCoordinateDescriptionType;

            ShipDescriptions.ForEach(shipDescription =>
            {
                shipDescription.Size = Clamp(shipDescription.Size, 1, 10);
                shipDescription.Count = Clamp(shipDescription.Count, 0, 20);
            });
        }
    }
}
