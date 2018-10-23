using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;
using FluentAssertions;

namespace Tests
{
    public class GameTests
    {
        [Fact]
        public void CountsNoLiveNeighborsFor_0x0_In_1x1_Grid()
        {
            var grid = new[,]
            {
                {false},
            };

            CountLiveNeighbors(grid, 0, 0).Should().Be(0);
        }

        [Fact]
        public void CountsOneLiveNeighborsFor_0x0_In_2x2_Grid()
        {
            var grid = new[,]
            {
                {false,false},
                {false,true},
            };

            CountLiveNeighbors(grid, 0, 0).Should().Be(1);
        }

        [Fact]
        public void CountsTwoLiveNeighborsFor_0x0_In_2x2_Grid()
        {
            var grid = new[,]
            {
                {false,true},
                {true,false},
            };

            CountLiveNeighbors(grid, 0, 0).Should().Be(2);
        }

        [Fact]
        public void CountsTwoLiveNeighborsFor_1x1_In_2x2_Grid()
        {
            var grid = new[,]
            {
                {false,true},
                {true,false},
            };

            CountLiveNeighbors(grid, 1, 1).Should().Be(2);
        }

        [Fact]
        public void CountsOneLiveNeighborsFor_1x1_In_2x2_Grid()
        {
            var grid = new[,]
            {
                {true,false},
                {false,false},
            };

            CountLiveNeighbors(grid, 1, 1).Should().Be(1);
        }

        [Fact]
        public void AnyLiveCellWithFewerThanTwoLiveNeighborsDies()
        {
            var grid = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true,  false, false},
                {false, false, true,  false, false},
                {false, false, true,  false, false},
                {false, false, false, false, false},
            };

            var newGrid = (bool[,])grid.Clone();

            for (int x = grid.GetLowerBound(0); x < grid.GetUpperBound(0); x++)
            {
                for (int y = grid.GetLowerBound(0); y < grid.GetUpperBound(1); y++)
                {
                    if (!grid[x, y])
                    {
                        continue;
                    }

                    newGrid[x, y] = CountLiveNeighbors(grid, x, y) >= 2;
                }
            }

            newGrid.Should().BeEquivalentTo(new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false,  false, false},
                {false, false, true,  false, false},
                {false, false, false,  false, false},
                {false, false, false, false, false},
            });
        }

        private int CountLiveNeighbors(bool[,] testGrid, int x, int y)
        {
            var xUpper = testGrid.GetUpperBound(0);
            var xLower = testGrid.GetLowerBound(0);
            var yUpper = testGrid.GetUpperBound(1);
            var yLower = testGrid.GetLowerBound(1);

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

                    if (testGrid[iX, iY])
                        liveCount++;
                }
            }

            return liveCount;
        }
    }
}
