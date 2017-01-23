using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using The2048Game.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using The2048Game.Models;
using The2048Game.Extensions;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace The2048Game.Controllers
{
    public class GameController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public GameController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: /Game/
        public IActionResult Index()
        {
            var game = HttpContext.Session.GetObjectFromJson<Game>("Game");
            if (game == null)
            {
                game = new Game()
                {
                    State = new int?[4, 4] { { 4096, 2048, 1024, 512 }, { 256, 128, 64, 32 }, { 16, 8, 4, 2 }, { null, 2, 4, null } },
                    DoubleUpState = new bool[4, 4] { { false, false, false, false }, { false, false, false, false }, { false, false, false, false }, { false, false, false, false } },
                    Score = 0,
                    Highscore = 0
                };
            }

            var model = new GameViewModel()
            {
                Game = game
            };

            HttpContext.Session.SetObjectAsJson("Game", game);

            return View(model);
        }

        // POST: /Game/NextMove/
        [HttpPost]
        public ActionResult NextMove([FromBody] string move)
        {
            var game = HttpContext.Session.GetObjectFromJson<Game>("Game");
            if (game == null)
            {
                game = new Game()
                {
                    State = new int?[4, 4] { { 4096, 2048, 1024, 512 }, { 256, 128, 64, 32 }, { 16, 8, 4, 2 }, { null, 2, 4, null } },
                    Score = 0,
                    Highscore = 0
                };
            }
            else
            {
                game = CalculateGameState(game, move);
            }

            var model = new GameViewModel()
            {
                Game = game
            };

            return PartialView("_GameCube");
        }

        private Game CalculateGameState(Game game, string type)
        {
            //type = "up"
            for (int y = 0; y < game.State.GetLength(1) - 1; y++) //kolommen
            {
                var x = 0;
                while (x < game.State.GetLength(0) - 1) //rijen
                {
                    if (game.State[x, y] != null && game.State[x, y] == game.State[x + 1, y])
                    {
                        game.State[x, y] = (game.State[x, y] ?? 0) + (game.State[x + 1, y] ?? 0);
                        game.State[x + 1, y] = null;
                    }

                    x++;
                }

                x = 0;
                var begin = 0;
                var gaps = false;
                while (true)
                {
                    while (begin < game.State.GetLength(0) - 1)

                    if (!gaps)
                        break;
                    else
                        x++;
                }
            }

            return game;
        }
    }
}
