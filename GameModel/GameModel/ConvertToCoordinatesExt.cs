namespace GameModel
{
    public static class ConvertToCoordinatesExt
    {
        public static Tuple<string, string>? ConvertToCoordinates(this string input)
        {
            string coordinates = input.Trim();
            string xCoor = "";
            string yCoor = "";

            if (coordinates.Length < 2 || coordinates.Length > 3)
                return null;

            if (char.IsLetter(coordinates[0]))
            {
                xCoor += coordinates[0];
                if (char.IsDigit(coordinates[1]))
                    yCoor = ReadNumber(coordinates, 1);
            }
            else if (char.IsDigit(coordinates[0]))
            {
                xCoor = ReadNumber(coordinates, 0);
                if (coordinates.Length > xCoor.Length && char.IsLetter(coordinates[xCoor.Length]))
                {
                    yCoor += coordinates[xCoor.Length];
                }
            }
            if (xCoor.Length > 0 && yCoor.Length > 0 && xCoor.Length + yCoor.Length == coordinates.Length)
                return Tuple.Create(xCoor, yCoor);
            else
                return null;


            // Reads up to 2 digit number
            static string ReadNumber(string coor, int startIndex)
            {
                string result = "";
                for (int i = startIndex; i <= startIndex + 1 && i < coor.Length && char.IsDigit(coor[i]); i++)
                    result += coor[i];
                return result;
            }
        }
    }
}
