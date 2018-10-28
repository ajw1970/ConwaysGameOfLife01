using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tests
{
    public class GameTests
    {
        private readonly ITestOutputHelper output;

        public GameTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void CanCreateGameBoardWithSimpleSeed()
        {
            var board = new GameBoard("0");

            board.GetPrintout().Should().Be("-");
        }

        [Fact]
        public void CanCycleGameOnce()
        {
            var seed = new StringBuilder();
            seed.AppendLine("-----");
            seed.AppendLine("--1--");
            seed.AppendLine("--1--");
            seed.AppendLine("--1--");
            seed.AppendLine("-----");

            var board = new GameBoard(seed.ToString());
            board.Cycle();

            var expected = new StringBuilder();
            expected.AppendLine("-----");
            expected.AppendLine("-----");
            expected.AppendLine("-111-");
            expected.AppendLine("-----");
            expected.AppendLine("-----");

            board.GetPrintout().Should().Be(expected.ToString().TrimEnd());
        }

        [Fact]
        public void CountsNoLiveNeighborsFor_0x0_In_1x1_Grid()
        {
            var board = new GameBoard("-");

            board.CountLiveNeighborsForCellPosition(0, 0).Should().Be(0);
        }

        [Fact]
        public void CountsOneLiveNeighborsFor_0x0_In_2x2_Grid()
        {
            var seed = new StringBuilder();
            seed.AppendLine("--");
            seed.AppendLine("-1");
            var board = new GameBoard(seed.ToString());

            board.CountLiveNeighborsForCellPosition(0, 0).Should().Be(1);
        }

        [Fact]
        public void CountsTwoLiveNeighborsFor_0x0_In_2x2_Grid()
        {
            var seed = new StringBuilder();
            seed.AppendLine("-1");
            seed.AppendLine("1-");
            var board = new GameBoard(seed.ToString());

            board.CountLiveNeighborsForCellPosition(0, 0).Should().Be(2);
        }

        [Fact]
        public void CountsTwoLiveNeighborsFor_1x1_In_2x2_Grid()
        {
            var seed = new StringBuilder();
            seed.AppendLine("-1");
            seed.AppendLine("1-");
            var board = new GameBoard(seed.ToString());

            board.CountLiveNeighborsForCellPosition(1, 1).Should().Be(2);
        }

        [Fact]
        public void CountsOneLiveNeighborsFor_1x1_In_2x2_Grid()
        {
            var seed = new StringBuilder();
            seed.AppendLine("1-");
            seed.AppendLine("--");
            var board = new GameBoard(seed.ToString());

            board.CountLiveNeighborsForCellPosition(1, 1).Should().Be(1);
        }

        [Fact]
        public void LiveCellWithFewerThanTwoLiveNeighborsDies()
        {
            GameBoard.GetNewCellState(true, 0).Should().BeFalse();
            GameBoard.GetNewCellState(true, 1).Should().BeFalse();
        }

        [Fact]
        public void LiveCellWithTwoOrThreeLiveNeighborsLives()
        {
            GameBoard.GetNewCellState(true, 2).Should().BeTrue();
            GameBoard.GetNewCellState(true, 3).Should().BeTrue();
        }

        [Fact]
        public void LiveCellWithMoreThanThreeLiveNeighborsDies()
        {
            GameBoard.GetNewCellState(true, 4).Should().BeFalse();
        }

        [Fact]
        public void DeadCellWithThreeLiveNeighborsBecomesLive()
        {
            GameBoard.GetNewCellState(false, 3).Should().BeTrue();
        }

        [Fact]
        public void DeadCellWithoutThreeLiveNeighborsBecomesLive()
        {
            GameBoard.GetNewCellState(false, 2).Should().BeFalse();
            GameBoard.GetNewCellState(false, 4).Should().BeFalse();
        }

        [Fact]
        public void BlinkerTest()
        {
            var seed = new StringBuilder();
            seed.AppendLine("-----");
            seed.AppendLine("--1--");
            seed.AppendLine("--1--");
            seed.AppendLine("--1--");
            seed.AppendLine("-----");
            var board = new GameBoard(seed.ToString());

            board.Cycle();

            var expected = new StringBuilder();
            expected.AppendLine("-----");
            expected.AppendLine("-----");
            expected.AppendLine("-111-");
            expected.AppendLine("-----");
            expected.AppendLine("-----");

            board.GetPrintout().Should().Be(expected.ToString().TrimEnd());
        }

        [Fact]
        public void ToadTest()
        {
            var seed = new StringBuilder();
            seed.AppendLine("------");
            seed.AppendLine("------");
            seed.AppendLine("--111-");
            seed.AppendLine("-111--");
            seed.AppendLine("------");
            seed.AppendLine("------");
            var board = new GameBoard(seed.ToString());

            board.Cycle();

            var expected = new StringBuilder();
            expected.AppendLine("------");
            expected.AppendLine("---1--");
            expected.AppendLine("-1--1-");
            expected.AppendLine("-1--1-");
            expected.AppendLine("--1---");
            expected.AppendLine("------");

            board.GetPrintout().Should().Be(expected.ToString().TrimEnd());
        }
    }

    public class GameBoard
    {
        private bool[,] grid;

        public GameBoard(string seedPattern)
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
                    {
                        continue;
                    }

                    if (grid[iX, iY])
                        liveCount++;
                }
            }

            return liveCount;
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

        public static bool GetNewCellState(bool living, int livingNeighbors)
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

        public string GetPrintout()
        {
            var sb = new StringBuilder();
            for (int x = grid.GetLowerBound(0); x <= grid.GetUpperBound(0); x++)
            {
                for (int y = grid.GetLowerBound(0); y <= grid.GetUpperBound(1); y++)
                {
                    sb.Append(grid[x, y] ? "1" : "-");
                }

                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }
    }

}
