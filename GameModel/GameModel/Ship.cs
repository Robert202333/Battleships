using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{

    public class ShipComponent
    {
        public bool WasHit { get; set; } = false;
        public Ship Ship { get; init; }
        public Coordinates Coordinates{ get; init;}

        internal ShipComponent(Ship ship, Coordinates coordinates)
        {
            Ship = ship;
            Coordinates = coordinates;
        }
    }


    public class Ship
    {
        public ShipComponent[] Components { get; init; }

        public string Name { get; init; }

        public bool IsSunk
        {
            get { return Components.All(component => component.WasHit); }
        }

        internal Ship(string name, IEnumerable<Coordinates> coordinates)
        {
            Name = name;
            Components = coordinates.Select(coor => new ShipComponent(this, coor)).ToArray();
        }
    }
}
