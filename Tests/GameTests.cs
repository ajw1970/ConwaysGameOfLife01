using System.Text;
using Xunit;
using FluentAssertions;

namespace Tests
{
    public class GameTests
    {
        [Fact]
        public void CanCreateGameBoardWithSimpleSeed()
        {
            var board = new GameBoard("0");

            board.GetPrintout().Should().Be("-");
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
}
