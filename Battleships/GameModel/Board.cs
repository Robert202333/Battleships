using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.GameModel
{
    internal class CoordinateDescriptor
    {
        private readonly string[] descriptions;
        private readonly Dictionary<string, uint> descriptionToIndexMap = new Dictionary<string, uint>();

        public uint Size
        {
            get { return (uint)descriptions.Length; }
        }

        internal CoordinateDescriptor(IEnumerable<string> descriptions)
        {
            this.descriptions = descriptions.ToArray();
            for (uint i = 0; i < this.descriptions.Length; i++)
            {
                descriptionToIndexMap.Add(this.descriptions[i], i);
            }
        }


        internal string GetDescription(uint index)
        {
            if (index > descriptions.Length)
                throw new ArgumentOutOfRangeException();

            return descriptions[index];
        }

        internal uint GetCoordinate(string description)
        {
            bool success = descriptionToIndexMap.TryGetValue(description, out var coordinateValue);
            if (success)
                return coordinateValue;

            throw new InvalidIndexDescriptionException(CreateDescriptionOutOfRangeMessage(description));
        }


        private string CreateDescriptionOutOfRangeMessage(string passedDescription)
        {
            string validDescriptions = string.Join(",", descriptions);
            return $"Passed {passedDescription} is not a valid index, please use one of the following: {validDescriptions}";
        }
    }

    internal class Square
    {
        internal ShipComponent? ShipComponenrt { get; set; } = null;
        internal bool WasHit { get; set; } = false;

        internal Square()
        {
        }
    }

    internal class Board
    {
        private readonly Square[,] squares;
        internal CoordinateDescriptor VerticalDescriptior { get; init; }
        internal CoordinateDescriptor HorizontalDescriptor { get; init; }

        private Square GetSquare(uint x, uint y)
        {
            return squares[y, x];
        }

        internal Board(CoordinateDescriptor horizontalDescriptor, CoordinateDescriptor verticalDescriptor)
        {
            VerticalDescriptior = verticalDescriptor;
            HorizontalDescriptor = horizontalDescriptor;

            squares = new Square[VerticalDescriptior.Size, HorizontalDescriptor.Size];
            for (int y = 0; y < squares.GetLength(0); y++)
                for (int x = 0; x < squares.GetLength(1); x++)
                    squares[y, x] = new Square();
        }

        internal Tuple<uint, uint> ConvertToCoordinates(string xDescription, string yDescription)
        {
            return new Tuple<uint, uint>(HorizontalDescriptor.GetCoordinate(xDescription), 
                                         VerticalDescriptior.GetCoordinate(yDescription));
        }
        internal ShipComponent? ProcessShot(uint x, uint y)
        {
            if (y > squares.GetLength(0) || x > squares.GetLength(1))
                throw new ArgumentOutOfRangeException();

            var square = GetSquare(x, y);
            if (square.WasHit)
                throw new RepeatedHitException();

            square.WasHit = true;
            if (square.ShipComponenrt != null)
                square.ShipComponenrt.WasHit = true;

            return square.ShipComponenrt;
        }

        internal void ApplyShipComponents(IEnumerable<ShipComponent> components)
        {
            foreach(var component in components)
            {
                var coordinates = component.Coordinates;
                var square = GetSquare(coordinates.X, coordinates.Y);
                square.ShipComponenrt = component;
            }
        }

        internal void VisitSquares(Action<Square, uint, uint> action, Predicate<Square>? predicate = null)
        {
            for (uint y = 0; y < squares.GetLength(0); y++)
                for (uint x = 0; x < squares.GetLength(1); x++)
                {
                    var square = GetSquare(x, y);
                    if (predicate == null || predicate(square))
                        action(square, x, y);
                }
        }
    }
}
