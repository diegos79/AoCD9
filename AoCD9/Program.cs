using System;
using System.IO;


namespace AOCD9
{

    public class Points
    {
        public int PointValue { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }

        //public int CountBasin { get; set; }

        public Points(int pointValue, int coordX, int coordY)
        {
            PointValue = pointValue;
            CoordX = coordX;
            CoordY = coordY;
        }


        public override bool Equals(object other)
        {
            Points otherItem = other as Points;

            if (otherItem == null)
                return false;

            return CoordX == otherItem.CoordX && CoordY == otherItem.CoordY;
        }
        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + CoordX.GetHashCode();
            hash = (hash * 7) + CoordY.GetHashCode();
            return hash;
        }

    }

    public class Program
    {
        public static void Main()
        {

            var stringArray = ConvertInputToStringArray("input.txt");
            var matrix = SetMatrix(stringArray);
            PrintMatrix(matrix);
            var lowPoints = GetLowPoints(matrix);
            var risk = GetRiskLevel(lowPoints);
            Console.WriteLine($"The risk level is: {risk}");

            foreach (var item in lowPoints) 
            {
                Console.Write($"Coordinates of the Low Points {item.PointValue}: ");
                Console.WriteLine(item.CoordX.ToString()+" "+ item.CoordY.ToString());
            }
            var basinPoints = GetBasinPoints(matrix, lowPoints).Distinct().ToList();
            var countBasin = CountBasin(matrix, lowPoints).Distinct().ToDictionary(t => t.Key, t => t.Value);
            Console.WriteLine();

            foreach (var item in basinPoints)
            {
                Console.WriteLine($"Low Point Basin: {item.PointValue} X: {item.CoordX} Y: {item.CoordY}");
            }

            foreach (var item in countBasin)
            {
                Console.WriteLine($"LowPoint {item.Key.PointValue} has a basin of ---> {item.Value} elements");

            }

            Console.WriteLine($"Total low basin points excluding lowpoints: {basinPoints.Count}");
            var top3 = countBasin.OrderByDescending(o => o.Value).Take(3).Select(x => x.Value).ToList();
            long finalResult=1;
            foreach (var item in top3)
            {
                finalResult *= item;
            }
            Console.WriteLine($"Final result is: \t{finalResult}");
        }


        public static string[] ConvertInputToStringArray (string input)
        {
            return File.ReadAllLines(input);
        }

        public static int[,] SetMatrix(string[] input)
        {
            int[,] result = new int[input.GetLength(0), input[0].Length];
            for (int row = 0; row < input.GetLength(0); row++)
            {
                for (int col = 0; col < input[0].Length; col++)
                {
                    result[row, col] = int.Parse(input[row][col].ToString());
                }

            }
            return result;
        }

        public static void PrintMatrix(int[,] matrix)
        {
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    Console.Write(matrix[row,col]);
                }
                Console.WriteLine();
            }
        }
        
        public static bool CheckLowPoint (int[,] matrix, int x, int y)
        {
            var up = x - 1;
            var down = x + 1;
            var left = y - 1;
            var right = y + 1;
            if (x == 0) up = 1;
            if (y == 0) left = 1;
            if (y == matrix.GetLength(1)-1) right = matrix.GetLength(1) - 2;
            if (x == matrix.GetLength(0)-1) down = matrix.GetLength(0) - 2;


            if (matrix[x, y] < matrix[up, y] &&
                matrix[x, y] < matrix[down, y] &&
                matrix[x, y] < matrix[x, left] &&
                matrix[x, y] < matrix[x, right]
                )
                return true;
            else return false;
        }


        public static List<Points> GetLowPointsBasinLeft(int[,] matrix, int x, int y)
        {
            List<Points> result = new List<Points>();
            var left = y - 1;
            int xInitialState = x;
            int yInitialState = y;
            if (y > 0)
                left = y - 1;
            else left = y;
            while (matrix[x, left] != 9 && left >= 0)
            {
                if (matrix[x, left] > matrix[x, y] || matrix[x, left] != 9)
                {
                    result.Add(new Points(matrix[xInitialState, yInitialState], x, left));
                    result.AddRange(GetLowPointsBasinDown(matrix,x,left));
                    result.AddRange(GetLowPointsBasinUp(matrix,x,left));

                }

                if (left == 0 || y == 0) break;
                    else { left--; y--; }
            }
            return result;
        }

        public static List<Points> GetLowPointsBasinRight(int[,] matrix, int x, int y)
        {
            List<Points> result = new List<Points>();
            var right = y + 1;
            int xInitialState = x;
            int yInitialState = y;
            if (y < matrix.GetLength(1) - 1)
                right = y + 1;
            else right = y;

            while (matrix[x, right] != 9 && right <= matrix.GetLength(1) - 1)
            {
                if (matrix[x, right] > matrix[x, y])
                {
                    result.Add(new Points(matrix[xInitialState, yInitialState], x, right));
                    result.AddRange(GetLowPointsBasinDown(matrix, x, right));
                    result.AddRange(GetLowPointsBasinUp(matrix, x, right));
                }
                if (right == matrix.GetLength(1) - 1 || y == matrix.GetLength(1) - 1) break;
                else { right++; y++; }
            }
            return result;
        }

        public static List<Points> GetLowPointsBasinUp(int[,] matrix, int x, int y)
        {
            List<Points> result = new List<Points>();
            int up = x - 1;
            int xInitialState = x;
            int yInitialState = y;
            if (x > 0)
                up = x - 1;
            else up = x;
            while (matrix[up, y] != 9 && up >= 0)
            {
                if (matrix[up, y] > matrix[x, y])
                {
                    result.Add(new Points(matrix[xInitialState, yInitialState], up, y)); //lowPointsBasin on the top
                    result.AddRange(GetLowPointsBasinLeft(matrix, up, y));
                    result.AddRange(GetLowPointsBasinRight(matrix, up, y));
                }
                if (up == 0 || x == 0) break;
                else { up--; x--; }
            }
            return result;
        }

        public static List<Points> GetLowPointsBasinDown(int[,] matrix, int x, int y)
        {
            List<Points> result = new List<Points>();
            int down = x + 1;
            int xInitialState = x;
            int yInitialState = y;
            if (x < matrix.GetLength(0) - 1)
                down = x + 1;
            else down = x;
            while (matrix[down, y] != 9 && down <= matrix.GetLength(0) - 1)
            {
                if (matrix[down, y] > matrix[x, y])
                {
                    result.Add(new Points(matrix[xInitialState, yInitialState], down, y)); //lowPointsBasin on the bottom
                    result.AddRange(GetLowPointsBasinLeft(matrix, down, y));
                    result.AddRange(GetLowPointsBasinRight(matrix, down, y));
                }
                if (down == matrix.GetLength(0) - 1 || x == matrix.GetLength(0) - 1) break;
                else { down++; x++; }
            }
            return result;
        }

        public static Dictionary<Points,int> CountBasin(int[,] matrix, List<Points> lowPoints)  //exluding lowpoints
        {
            List<Points> result = new List<Points>();
            Dictionary<Points, int> resultCount = new Dictionary<Points, int>();
            foreach (var point in lowPoints)
            {
                int x = point.CoordX;
                int y = point.CoordY;
                int resultcount = result.Distinct().Count();

                //--------------- lowpoint basin on the left ------------------------------

                result.AddRange(GetLowPointsBasinLeft(matrix, x, y));

                //----------------lowpoint basin on the right------------------------------
                result.AddRange(GetLowPointsBasinRight(matrix, x, y));

                //----------------lowpoint basin on up-------------------------------------

                result.AddRange(GetLowPointsBasinUp(matrix, x, y));

                //----------------lowpoint basin on down-----------------------------------

                result.AddRange(GetLowPointsBasinDown(matrix, x, y));


                int tempresult = result.Distinct().Count() - resultcount;
                resultCount[point] = tempresult + 1;
            }
            return resultCount;
        }


        public static List<Points> GetBasinPoints(int[,] matrix, List<Points> lowPoints)  //exluding lowpoints
        {
            List<Points> result = new List<Points>();
            Dictionary<Points, int> resultCount = new Dictionary<Points, int>();
            //List<Points> resultCount = new List<Points>();
            foreach (var point in lowPoints)  
            {
                var x = point.CoordX;
                var y = point.CoordY;

                //--------------- lowpoint basin on the left ------------------------------

                result.AddRange(GetLowPointsBasinLeft(matrix, x, y));

                //----------------lowpoint basin on the right------------------------------
                result.AddRange(GetLowPointsBasinRight(matrix, x, y));

                //----------------lowpoint basin on up-------------------------------------

                result.AddRange(GetLowPointsBasinUp(matrix, x, y));

                //----------------lowpoint basin on down-----------------------------------

                result.AddRange(GetLowPointsBasinDown(matrix, x, y));

                
            }
            return result;
        }
        
        public static List<Points> GetLowPoints (int[,] matrix)
        {
            List<Points> list = new List<Points>();
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    if (CheckLowPoint(matrix, row, col)) list.Add(new Points(matrix[row, col], row, col));
                }
            }
            return list;
        }


        public static int GetRiskLevel (List<Points> list)
        {
            int result = 0;
            foreach (var item in list)
            {
                result += item.PointValue+1;
            }
            return result;
        }

    }
}