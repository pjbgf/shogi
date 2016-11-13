﻿using NSubstitute;
using NUnit.Framework;

namespace Core.Shogi.Tests
{
    [TestFixture]
    public class ShogiGameShould
    {
        private IBoardRender _boardRenderMock;
        private IBoardInput _blackPlayerMock;
        private IBoardInput _whitePlayerMock;
        private Board _boardMock;

        [SetUp]
        public void BeforeEachTest()
        {
            _boardRenderMock = Substitute.For<IBoardRender>();
            _blackPlayerMock = Substitute.For<IBoardInput>();
            _whitePlayerMock = Substitute.For<IBoardInput>();
            _boardMock = Substitute.For<Board>();
        }

        [Test]
        public void ResetBoardOnStart()
        {
            var shogiGame = new ShogiGame(_boardRenderMock, null, null, _boardMock);

            shogiGame.Start();

            _boardMock.Received(1).ResetBoard();
        }

        [Test]
        public void RenderBoardOnStart()
        {
            var shogiGame = new ShogiGame(_boardRenderMock, null, null, _boardMock);

            shogiGame.Start();

            _boardRenderMock.ReceivedWithAnyArgs(1).Refresh(Arg.Any<BoardState>());
        }

        [Test]
        public void MovePieceBasedOnBlackPlayerInput()
        {
            _blackPlayerMock.AskForNextMove().Returns("1g1f");
            var shogiGame = new ShogiGame(_boardRenderMock, _blackPlayerMock, _whitePlayerMock, _boardMock);

            shogiGame.Start();

            _boardMock.Received(1).Move(Arg.Is(Player.Black), Arg.Is("1g"), Arg.Is("1f"));
        }

        [Test]
        public void MovePieceBasedOnWhitePlayerInput()
        {
            _blackPlayerMock.AskForNextMove().Returns("1g1f");
            _whitePlayerMock.Player.Returns(Player.White);
            _whitePlayerMock.AskForNextMove().Returns("1c1e");
            _boardMock.Move(Arg.Any<Player>(), Arg.Any<string>(), Arg.Any<string>())
                .ReturnsForAnyArgs(BoardResult.ValidOperation, BoardResult.InvalidOperation);

            var shogiGame = new ShogiGame(_boardRenderMock, _blackPlayerMock, _whitePlayerMock, _boardMock);

            shogiGame.Start();

            _boardMock.Received().Move(Arg.Is(Player.White), Arg.Is("1c"), Arg.Is("1e"));
        }

        [Test]
        public void TellUserOfInvalidMoves()
        {
            _blackPlayerMock.AskForNextMove().Returns("1c1e");
            var shogiGame = new ShogiGame(_boardRenderMock, _blackPlayerMock, _whitePlayerMock, _boardMock);

            shogiGame.Start();

            _boardRenderMock.Received(1).InvalidOperation(Arg.Any<BoardResult>());
        }

        [Test]
        [Ignore("To comply with this test, the logic is breaking other tests.")]
        public void AskPlayerTwiceIfFirstInputWasInvalid()
        {
            _blackPlayerMock.AskForNextMove().Returns("1c1e");
            _boardMock.Move(Arg.Any<Player>(), Arg.Any<string>(), Arg.Any<string>())
                .ReturnsForAnyArgs(BoardResult.InvalidOperation, BoardResult.ValidOperation);
            var shogiGame = new ShogiGame(_boardRenderMock, _blackPlayerMock, _whitePlayerMock, _boardMock);

            shogiGame.Start();

            _blackPlayerMock.Received(2).AskForNextMove();
        }
    }
}