using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{

    public class ShipComponent
    {
        public bool WasHit { get; internal set; } = false;
        public Ship Ship { get; init; }
        public Square Square { get; init; }

        internal ShipComponent(Ship ship, Square square)
        {
            Ship = ship;
            Square = square;
        }

    }


    public class Ship
    {
        public ShipComponent[] Components { get; private set; } = { };

        public string Name { get; init; }

        public bool WasSunk
        {
            get { return Components.All(component => component.WasHit); }
        }

        internal Ship(string name)
        {
            Name = name;            
        }

        internal void AddComponents(IEnumerable<ShipComponent> components)
        {
            Components = components.ToArray();
        }
    }
}
