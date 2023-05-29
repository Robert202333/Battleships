using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.GameModel
{
    public class RepeatedHitException : Exception
    {
        const string message = "The same square hit again";
        public RepeatedHitException() : base(message)
        {
        }
    }

    public class InvalidDescriptionException : Exception
    {
        public InvalidDescriptionException() : base()
        {
        }
    }

    public class InvalidIndexDescriptionException : Exception
    {        
        public InvalidIndexDescriptionException(string message) : base(message)
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
