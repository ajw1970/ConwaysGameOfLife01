using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tests
{
    public class GameBoard
    {
        private bool[,] grid;

        public GameBoard(string seedPattern)
        {
            MapSeedToGrid(ParseSeedString(seedPattern));
        }

        private static List<List<bool>> ParseSeedString(string seedPattern)
        {
            var cellLines = new List<List<bool>>();

            using (var reader = new StringReader(seedPattern))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;

                    var cellLine = new List<bool>();
                    foreach (var chr in line)
                    {
                        cellLine.Add(chr == '1');
                    }

                    cellLines.Add(cellLine);
                }
            }

            return cellLines;
        }

        private void MapSeedToGrid(List<List<bool>> cellLines)
        {
            if (cellLines.GroupBy(c => c.Count).Count() > 1)
                throw new ArgumentException("Seed must have equal rows");

            grid = new bool[cellLines.First().Count, cellLines.Count];

            for (var x = 0; x < cellLines.Count; x++)
            {
                for (var y = 0; y < cellLines.First().Count; y++)
                {
                    grid[x, y] = cellLines[x][y];
                }
            }
        }

        public void Cycle()
        {
            var newGrid = (bool[,])grid.Clone();

            for (int x = grid.GetLowerBound(0); x <= grid.GetUpperBound(0); x++)
            {
                for (int y = grid.GetLowerBound(0); y <= grid.GetUpperBound(1); y++)
                {
                    var livingNeighbors = CountLiveNeighborsForCellPosition(x, y);
                    newGrid[x, y] = GetNewCellState(grid[x, y], livingNeighbors);
                }
            }

            grid = newGrid;
        }

        public string GetPrintout()
        {
            var sb = new StringBuilder();
            for (int x = grid.GetLowerBound(0); x <= grid.GetUpperBound(0); x++)
            {
                for (int y = grid.GetLowerBound(0); y <= grid.GetUpperBound(1); y++)
                    sb.Append(grid[x, y] ? "1" : "-");

                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        public int CountLiveNeighborsForCellPosition(int x, int y)
        {
            var xUpper = grid.GetUpperBound(0);
            var xLower = grid.GetLowerBound(0);
            var yUpper = grid.GetUpperBound(1);
            var yLower = grid.GetLowerBound(1);

            int liveCount = 0;
            for (int iX = x - 1; iX <= x + 1; iX++)
            {
                if (iX < xLower || iX > xUpper)
                    continue;

                for (int iY = y - 1; iY <= y + 1; iY++)
                {
                    if (iY < yLower || iY > yUpper)
                        continue;

                    if (iX == x && iY == y)
                        continue;

                    if (grid[iX, iY])
                        liveCount++;
                }
            }

            return liveCount;
        }

        public  static bool GetNewCellState(bool living, int livingNeighbors)
        {
            if (living)
            {
                if (livingNeighbors < 2)
                    return false;

                if (livingNeighbors >= 2 && livingNeighbors <= 3)
                    return true;

                if (livingNeighbors > 3)
                    return false;

                throw new ArgumentOutOfRangeException($"{nameof(livingNeighbors)} must be positive");
            }

            if (livingNeighbors == 3)
                return true;

            return false;
        }
    }
}