namespace TicTacToe.WebApi.Controllers
{
    using System.Web.Http;
    using Data.UnitOfWork;
    using Microsoft.AspNet.Identity;
    using TicTacToe.Models;
    using System.Linq;
    using System;
    using DataModel;
    using System.Text;
    using GameLogic;

    [Authorize]
    public class GamesController : ApiController
    {
        private ITicTacToeData data;
        private GameResultValidator validator;

        public GamesController(ITicTacToeData data, GameResultValidator validator)
        {
            this.data = data;
            this.validator = validator;
        }

        [HttpPost]
        public IHttpActionResult Create()
        {
            var currentUserId = this.User.Identity.GetUserId();

            var newGame = new Game
            {
                FirstPlayerId = currentUserId
            };

            this.data.Games.Add(newGame);
            this.data.SaveChanges();

            return this.Ok(newGame.Id);
        }

        [HttpPost]
        public IHttpActionResult Join()
        {
            var currentUserId = this.User.Identity.GetUserId();

            var game = this.data
                            .Games
                            .All()
                            .Where(ga => ga.State == GameState.WaitingForSecondPlayer && ga.FirstPlayerId != currentUserId)
                            .FirstOrDefault();

            if (game == null)
            {
                return this.NotFound();
            }

            game.SecondPlayerId = currentUserId;
            game.State = GameState.TurnX;
            this.data.SaveChanges();

            return this.Ok(game);
        }

        [HttpGet]
        public IHttpActionResult Status(string gameId)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var idAsGuid = new Guid(gameId);

            var game = this.data.Games
                                .All()
                                .Where(x => x.Id == idAsGuid)
                                .Select(x => new { x.FirstPlayerId, x.SecondPlayerId })
                                .FirstOrDefault();

            if (game == null)
            {
                return this.NotFound();
            }

            if (game.FirstPlayerId != currentUserId &&
                game.SecondPlayerId != currentUserId)
            {
                return this.BadRequest("You are not part of this game");
            }

            var gameInfo = this.data.Games
                                .All()
                                .Where(g => g.Id == idAsGuid)
                                .Select(g => new GameInfoDataModel()
                                {
                                    Board = g.Board,
                                    Id = g.Id,
                                    State = g.State,
                                    FirstPlayerName = g.FirstPlayer.UserName,
                                    SecondPlayerName = g.SecondPlayer.UserName
                                })
                                .FirstOrDefault();

            return this.Ok(gameInfo);
        }


        /// <param name="request">string gameId, row(range 1 to 3) and col(range 1 to 3)</param>
        [HttpPost]
        public IHttpActionResult Play(PlayRequestDataModel request)
        {
            var currentUserId = this.User.Identity.GetUserId();

            if (request == null || !this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var idAsGuid = new Guid(request.GameId);

            var game = this.data.Games.GetById(idAsGuid);

            if (game == null)
            {
                return this.BadRequest("Invalid game id!");
            }

            if (game.FirstPlayerId != currentUserId &&
                game.SecondPlayerId != currentUserId)
            {
                return this.BadRequest("You are not part of this game");

            }

            if (game.State != GameState.TurnX &&
                game.State != GameState.TurnY)
            {
                return this.BadRequest("INvalid game state!");
            }

            if ((game.State == GameState.TurnX && game.FirstPlayerId != currentUserId) ||
                (game.State == GameState.TurnY && game.SecondPlayerId != currentUserId))
            {
                return this.BadRequest("Its not your turn to play!");
            }

            var positionIndex = (request.Row - 1) * 3 + request.Col - 1;

            if (game.Board[positionIndex] != '-')
            {
                return this.BadRequest("Invalid position");
            }

            var sbBoard = new StringBuilder(game.Board);
            sbBoard[positionIndex] = game.State == GameState.TurnX ? 'X' : 'O';
            game.Board = sbBoard.ToString();
            game.State = game.State == GameState.TurnX ? GameState.TurnY : GameState.TurnX;

            this.data.SaveChanges();

            var gameResult = validator.GetResult(game.Board);
            switch (gameResult)
            {
                case GameResult.NotFinished:
                    
                    break;
                case GameResult.WonByX:
                    game.State = GameState.WonByX;
                    this.data.SaveChanges();
                    break;
                case GameResult.WonByY:
                    game.State = GameState.WonByY;
                    this.data.SaveChanges();
                    break;
                case GameResult.Draw:
                    game.State = GameState.Draw;
                    this.data.SaveChanges();
                    break;
                default:
                    break;
            }

            return this.Ok();
        }
    }
}
