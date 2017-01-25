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
                    State = new int?[4, 4] { { 8, 8, 4, 2 }, { 8, 4, 2, 2 }, { 4, 2, 2, 4 }, { 4, 2, null, 2 } },
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

        // GET: /Game/NextMove/
        public IActionResult NextMove(string move = "up")
        {
            var game = HttpContext.Session.GetObjectFromJson<Game>("Game");
            if (game == null)
            {
                game = new Game()
                {
                    State = new int?[4, 4] { { 8, 8, 4, 2 }, { 8, 4, 2, 2 }, { 4, 2, 2, 4 }, { 4, 2, null, 2 } },
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

            HttpContext.Session.SetObjectAsJson("Game", game);

            return PartialView("_GameCube", model);
        }

        private Game CalculateGameState(Game game, string type = "up")
        {
            switch (type)
            {
                case "up":
                    
                    for (int y = 0; y < game.State.GetLength(1); y++) //kolommen : van 0 tot 3
                    {
                        //deze lus zal de getallen verdubbelen indien ze gelijk zijn
                        for (int x = 0; x < game.State.GetLength(0) - 1; x++) //rijen : van 0 tot 2
                        {
                            int i = x + 1;

                            while (true)
                            {
                                if (game.State[x, y] != null)
                                {
                                    if (game.State[x, y] == game.State[i, y])
                                    {
                                        game.State[x, y] = (game.State[x, y] ?? 0) + (game.State[i, y] ?? 0);
                                        game.State[i, y] = null;
                                    }
                                    break;
                                }

                                i++;
                                if (i > game.State.GetLength(0) - 1)
                                {
                                    break;
                                }
                            }
                        }

                        //deze lus gaat alle lege gaten opvullen
                        for (int x = 0; x < game.State.GetLength(0) - 1; x++) // van 0 tot 2
                        {
                            if (game.State[x, y] == null)
                            {
                                int i = x + 1;
                                while (true)
                                {
                                    if (game.State[i, y] != null)
                                    {
                                        game.State[x, y] = game.State[i, y];
                                        game.State[i, y] = null;
                                        break;
                                    }

                                    i++;

                                    if (i > game.State.GetLength(0) - 1)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    break;

                case "down":

                    for (int y = 0; y < game.State.GetLength(1) ; y++) //kolommen : van 0 tot 3
                    {
                        //deze lus zal de getallen verdubbelen indien ze gelijk zijn
                        for (int x = game.State.GetLength(0) - 1; x >= 1; x--) //rijen : van 3 tot 1
                        {
                            int i = x - 1;

                            while (true)
                            {
                                if (game.State[x, y] != null)
                                {
                                    if (game.State[x, y] == game.State[i, y])
                                    {
                                        game.State[x, y] = (game.State[x, y] ?? 0) + (game.State[i, y] ?? 0);
                                        game.State[i, y] = null;
                                    }
                                    break;
                                }

                                i--;
                                if (i < 0)
                                {
                                    break;
                                }
                            }
                        }

                        //deze lus gaat alle lege gaten opvullen
                        for (int x = game.State.GetLength(0) - 1; x >= 1 ; x--) // van 3 tot 1
                        {
                            if (game.State[x, y] == null)
                            {
                                int i = x - 1;
                                while (true)
                                {
                                    if (game.State[i, y] != null)
                                    {
                                        game.State[x, y] = game.State[i, y];
                                        game.State[i, y] = null;
                                        break;
                                    }

                                    i--;

                                    if (i < 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    break;

                case "left":

                    for (int x = 0; x < game.State.GetLength(1); x++) //rijen : van 0 tot 3
                    {
                        //deze lus zal de getallen verdubbelen indien ze gelijk zijn
                        for (int y = 0; y < game.State.GetLength(0) - 1; y++) //kolommen : van 0 tot 2
                        {
                            int i = y + 1;

                            while (true)
                            {
                                if (game.State[x, y] != null)
                                {
                                    if (game.State[x, y] == game.State[x, i])
                                    {
                                        game.State[x, y] = (game.State[x, y] ?? 0) + (game.State[x, i] ?? 0);
                                        game.State[x, i] = null;
                                    }
                                    break;
                                }

                                i++;
                                if (i > game.State.GetLength(0) - 1)
                                {
                                    break;
                                }
                            }
                        }

                        //deze lus gaat alle lege gaten opvullen
                        for (int y = 0; y < game.State.GetLength(0) - 1; y++) // van 0 tot 2
                        {
                            if (game.State[x, y] == null)
                            {
                                int i = y + 1;
                                while (true)
                                {
                                    if (game.State[x, i] != null)
                                    {
                                        game.State[x, y] = game.State[x, i];
                                        game.State[x, i] = null;
                                        break;
                                    }

                                    i++;

                                    if (i > game.State.GetLength(0) - 1)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    
                    break;

                case "right":

                    for (int x = 0; x < game.State.GetLength(1); x++) //rijen : van 0 tot 3
                    {
                        //deze lus zal de getallen verdubbelen indien ze gelijk zijn
                        for (int y = game.State.GetLength(0) - 1; y >= 1; y--) //kolommen : van 3 tot 1
                        {
                            int i = y - 1;

                            while (true)
                            {
                                if (game.State[x, y] != null)
                                {
                                    if (game.State[x, y] == game.State[x, i])
                                    {
                                        game.State[x, y] = (game.State[x, y] ?? 0) + (game.State[x, i] ?? 0);
                                        game.State[x, i] = null;
                                    }
                                    break;
                                }

                                i--;
                                if (i < 0)
                                {
                                    break;
                                }
                            }
                        }

                        //deze lus gaat alle lege gaten opvullen
                        for (int y = game.State.GetLength(0) - 1; y >= 1; y--) // van 3 tot 1
                        {
                            if (game.State[x, y] == null)
                            {
                                int i = y - 1;
                                while (true)
                                {
                                    if (game.State[x, i] != null)
                                    {
                                        game.State[x, y] = game.State[x, i];
                                        game.State[x, i] = null;
                                        break;
                                    }

                                    i--;

                                    if (i < 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    break;

            }
            return game;

        }
    }
}
