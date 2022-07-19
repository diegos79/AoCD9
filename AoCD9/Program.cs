using System;
using System.IO;


namespace AOCD9
{

    public class LowPoints
    {
        public int LowPoint { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int BasinValue { get; set; }
        public LowPoints (int lowPoint, int coordX, int coordY)
        {
            LowPoint = lowPoint;
            CoordX = coordX;
            CoordY = coordY;
        }

        public LowPoints(int lowPoint, int coordX, int coordY, int basinValue)
        {
            LowPoint = lowPoint;
            CoordX = coordX;
            CoordY = coordY;
            BasinValue = basinValue;
        }


        public LowPoints() { }
    }

    public class Program
    {
        public static void Main()
        {
            var stringArray = ConvertInputToStringArray("sample.txt");
            var matrix = SetMatrix(stringArray);
            PrintMatrix(matrix);
            var lowPoints = GetLowPoints(matrix);
            var risk = GetRiskLevel(lowPoints);
            Console.WriteLine($"The risk level is: {risk}");
            var getLowPointsCoords = GetLowPointsCoords(matrix);
            foreach (var item in getLowPointsCoords)
            {
                Console.Write("Coordinates of the Low Points: ");
                Console.WriteLine(item.CoordX.ToString()+" "+ item.CoordY.ToString());
            }
            var y2 = GetLowPointsBasin(matrix, getLowPointsCoords);
            Console.WriteLine();
            foreach (var item in y2)
            {
                Console.WriteLine($"Low Point Basin on lowpoint: {item.LowPoint} X: {item.CoordX} Y: {item.CoordY} Basin: {item.BasinValue}");

            }
            
            Console.WriteLine($"Total low basin points: {y2.Count}");

            
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

        public static List<LowPoints> GetLowPointsBasinCenter(int[,] matrix, int x, int y)
        {
            List<LowPoints> result = new List<LowPoints>();
            var up = x - 1;
            var down = x + 1;
            var left = y - 1;
            var right = y + 1;

            if (x == 0) up = 1;
            if (y == 0) left = 1;
            if (y == matrix.GetLength(1) - 1) right = matrix.GetLength(1) - 2;
            if (x == matrix.GetLength(0) - 1) down = matrix.GetLength(0) - 2;

            if (matrix[x, y] < matrix[up, y] &&
                matrix[x, y] < matrix[down, y] &&
                matrix[x, y] < matrix[x, left] &&
                matrix[x, y] < matrix[x, right]
                )
                result.Add(new LowPoints(matrix[x,y], x, y, matrix[x, y])); //classic lowpoints adding to result list

            return result;
        }

        public static List<LowPoints> GetLowPointsBasinLeft(int[,] matrix, int x, int y)
        {
            List<LowPoints> result = new List<LowPoints>();
            var left = y - 1;
            while (matrix[x, left] != 9 && left >= 0)
            {
                if (matrix[x, left] > matrix[x, y] || matrix[x, left] != 9)
                {
                    //Console.WriteLine("Aggiungo un lowPoint Basin a sx");
                    result.Add(new LowPoints (matrix[x,y], x, left, matrix[x, left]));

                }
                if (left == 0 || y == 0) break;
                    else { left--; y--; }
            }
            return result;
        }

        public static List<LowPoints> GetLowPointsBasinRight(int[,] matrix, int x, int y)
        {
            List<LowPoints> result = new List<LowPoints>();
            var right = y + 1;
            if (y < matrix.GetLength(1) - 1)
                right = y + 1;
            else right = y;

            while (matrix[x, right] != 9 && right <= matrix.GetLength(1) - 1)
            {
                if (matrix[x, right] > matrix[x, y])
                {
                    result.Add(new LowPoints(matrix[x,y], x, right, matrix[x, right]));
                }
                if (right == matrix.GetLength(1) - 1 || y == matrix.GetLength(1) - 1) break;
                else { right++; y++; }
            }
            return result;
        }

        public static List<LowPoints> GetLowPointsBasinUp(int[,] matrix, int x, int y)
        {
            List<LowPoints> result = new List<LowPoints>();
            var up = x - 1;
            if (x > 0)
                up = x - 1;
            else up = x;
            while (matrix[up, y] != 9 && up >= 0)
            {
                if (matrix[up, y] > matrix[x, y])
                {
                    result.Add(new LowPoints(matrix[x,y], up, y, matrix[up, y])); //lowPointsBasin on the top
                }
                if (up == 0 || x == 0) break;
                else { up--; x--; }
            }
            return result;
        }

        public static List<LowPoints> GetLowPointsBasinDown(int[,] matrix, int x, int y)
        {
            List<LowPoints> result = new List<LowPoints>();
            var down = x + 1;
            if (x < matrix.GetLength(0) - 1)
                down = x + 1;
            else down = x;
            while (matrix[down, y] != 9 && down <= matrix.GetLength(0) - 1)
            {
                if (matrix[down, y] > matrix[x, y])
                {
                    //Console.WriteLine("Aggiungo un lowPoint Basin down");
                    result.Add(new LowPoints(matrix[x,y], down, y, matrix[down, y])); //lowPointsBasin on the bottom
                }
                if (down == matrix.GetLength(0) - 1 || x == matrix.GetLength(0) - 1) break;
                else { down++; x++; }
            }
            return result;
        }

        public static List<LowPoints> GetLowPointsBasin(int[,] matrix, List<LowPoints> lowPoints)
        {
            List<LowPoints> result = new List<LowPoints>();

            foreach (var item in lowPoints)
            {

                //--------------- lowpoint basin on center -------------------------------
                var x = item.CoordX;
                var y = item.CoordY;
                result.AddRange(GetLowPointsBasinCenter(matrix, x, y));

                //var up = x - 1;
                //var down = x + 1;
                //var left = y - 1;
                //var right = y + 1;

                //if (x == 0) up = 1;
                //if (y == 0) left = 1;
                //if (y == matrix.GetLength(1) - 1) right = matrix.GetLength(1) - 2;
                //if (x == matrix.GetLength(0) - 1) down = matrix.GetLength(0) - 2;

                //if (matrix[x, y] < matrix[up, y] &&
                //    matrix[x, y] < matrix[down, y] &&
                //    matrix[x, y] < matrix[x, left] &&
                //    matrix[x, y] < matrix[x, right]
                //    )
                //    result.Add(new LowPoints(item.LowPoint, x, y, matrix[x,y])); //classic lowpoints adding to result list


                //--------------- lowpoint basin on the left ------------------------------

                x = item.CoordX;
                y = item.CoordY;
                //while (matrix[x, left] != 9 && left >= 0)
                //{
                //    if (matrix[x, left] > matrix[x, y] || matrix[x, left] != 9)
                //    {
                //        //Console.WriteLine("Aggiungo un lowPoint Basin a sx");
                //        result.Add(new LowPoints(item.LowPoint, x, left, matrix[x, left]));

                //    }
                //    if (left == 0 || y == 0) break;
                //    else { left--; y--; }
                //}
                result.AddRange(GetLowPointsBasinLeft(matrix, x, y));

                //----------------lowpoint basin on the right------------------------------
                x = item.CoordX;
                y = item.CoordY;
                result.AddRange(GetLowPointsBasinRight(matrix, x, y));
                //if (y < matrix.GetLength(1) - 1)
                //    right = item.CoordY + 1;
                //else right = y;
                //while (matrix[x, right] != 9 && right < matrix.GetLength(1) - 1)
                //{
                //    if (matrix[x, right] > matrix[x, y])
                //    {
                //        result.Add(new LowPoints(item.LowPoint,x, right, matrix[x, right]));
                //            //lowPointsBasin on the right
                //    }
                //    if (right == matrix.GetLength(1) - 1 || y == matrix.GetLength(1) - 1) break;
                //    else { right++; y++; }
                //}

                //----------------lowpoint basin on up-------------------------------------


                x = item.CoordX;
                y = item.CoordY;
                result.AddRange(GetLowPointsBasinUp(matrix, x, y));
                                //if (x > 0)
                //    up = item.CoordX - 1;
                //else up = x;
                //while (matrix[up, y] != 9 && up > 0)
                //{
                //    if (matrix[up, y] > matrix[x, y])
                //    {
                //        //Console.WriteLine("Aggiungo un lowPoint Basin up");
                //        result.Add(new LowPoints(item.LowPoint, up, y, matrix[up, y])); //lowPointsBasin on the top
                //    }
                //    if (up == 0 || x == 0) break;
                //    else { up--; x--; }
                //}

                //----------------lowpoint basin on down-------------------------------------

                x = item.CoordX;
                y = item.CoordY;
                result.AddRange(GetLowPointsBasinDown(matrix, x, y));
                                //if (x < matrix.GetLength(0) - 1)
                //    down = item.CoordX + 1;
                //else down = x;
                //while (matrix[down, y] != 9 && down <= matrix.GetLength(0) - 1)
                //{
                //    if (matrix[down, y] > matrix[x, y])
                //    {
                //        //Console.WriteLine("Aggiungo un lowPoint Basin down");
                //        result.Add(new LowPoints(item.LowPoint, down, y, matrix[down, y])); //lowPointsBasin on the bottom
                //    }
                //    if (down == matrix.GetLength(0) - 1 || x == matrix.GetLength(0) - 1) break;
                //    else { down++; x++; }
                //}
            }
            return result;
        }
        

        public static List<int> GetLowPoints (int[,] matrix)
        {
            List<int> list = new List<int>();

            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    if (CheckLowPoint(matrix, row, col)) list.Add(matrix[row, col]);
                }
            }
            return list;
        }

        public static List<LowPoints> GetLowPointsCoords(int[,] matrix)
        {
            List<LowPoints> list = new List<LowPoints>();

            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    if (CheckLowPoint(matrix, row, col)) list.Add(new LowPoints { LowPoint = matrix[row, col], CoordX = row, CoordY = col });
                }
            }
            return list;
        }

        public static int GetRiskLevel (List<int> list)
        {
            int result = 0;
            foreach (var item in list)
            {
                result += item+1;
            }
            return result;
        }

    }
}