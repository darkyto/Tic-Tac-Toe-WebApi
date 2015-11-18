namespace TicTacToe.GameLogic
{
    using System;

    public class GameResultValidator : IGameResultValidator
    {
        public GameResult GetResult(string board)
        {
            return GameResult.NotFinished;
        }
    }
}
