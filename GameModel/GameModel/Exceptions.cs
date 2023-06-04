namespace GameModel
{
    public class InvalidCoordinatesDescriptionException : Exception
    {
        public InvalidCoordinatesDescriptionException(string message) : base(message)
        {
        }
    }

    public class CoordinateOutOfBoardException : Exception
    {
        public CoordinateOutOfBoardException(string message) : base(message)
        {
        }
    }


    public class ShipCreationException : Exception
    {
        const string message = "Can't place all ships on map. Try again, increase the map size or decrease the number of ships to create.";
        public ShipCreationException() : base(message)
        {
        }
    }

}
