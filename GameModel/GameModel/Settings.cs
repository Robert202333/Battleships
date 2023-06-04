namespace GameModel
{
    public enum CoordinateDescriptionType
    {
        Number,
        Letter,
    }

    public class ShipDescription
    {
        public string Name { get; set; } = "";
        public int Size { get; set; }
        public int Count { get; set; }
    }

    public class Settings
    {
        public int HorizontalSize { get; set; } = 10;
        public int VerticalSize { get; set; } = 10;

        public CoordinateDescriptionType HorizontalCoordinateDescriptionType { get; set; } = CoordinateDescriptionType.Letter;
        public CoordinateDescriptionType VerticalCoordinateDescriptionType { get; set; } = CoordinateDescriptionType.Number;

        public List<ShipDescription> ShipDescriptions { get; set; } = new()
        {
            new () { Name = "Destroyer", Count = 2, Size = 2},
            new () { Name = "Submarine", Count = 1, Size = 3},
            new () { Name = "Cruiser", Count = 1, Size = 4},
            new () { Name = "Battleship", Count = 1, Size = 5}
        };

        public bool StraightShips { get; set; } = true;
        public bool ShipsCanStick { get; set; } = false;

        public bool DebugMode { get; set; } = false;
        public Settings()
        {

        }

        // UI doesn't provude any validation. Called after leaving settings dialog
        public void Validate()
        {
            HorizontalSize = Math.Clamp(HorizontalSize, 5, 25);
            VerticalSize = Math.Clamp(VerticalSize, 5, 25);

            if (VerticalCoordinateDescriptionType == HorizontalCoordinateDescriptionType)
                HorizontalCoordinateDescriptionType = 1 - VerticalCoordinateDescriptionType;

            ShipDescriptions.ForEach(shipDescription =>
            {
                shipDescription.Size = Math.Clamp(shipDescription.Size, 1, 10);
                shipDescription.Count = Math.Clamp(shipDescription.Count, 0, 20);
            });
        }
    }
}
