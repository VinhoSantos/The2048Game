using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using The2048Game.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using The2048Game.Models;
using The2048Game.Extensions;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace The2048Game.Controllers
{
    public class GameController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public GameController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: /Game/
        public IActionResult Index()
        {
            var game = HttpContext.Session.GetObjectFromJson<Game>("Game") ?? StartNewGame();

            var model = new GameViewModel()
            {
                Game = game
            };

            HttpContext.Session.SetObjectAsJson("Game", game);

            return View(model);
        }

        public IActionResult Vue()
        {
            var game = HttpContext.Session.GetObjectFromJson<Game>("Game") ?? StartNewGame();

            var model = new GameViewModel()
            {
                Game = game
            };

            HttpContext.Session.SetObjectAsJson("Game", game);

            return View(model);
        }

        // GET: /Game/NextMove/
        public IActionResult NextMove(string move)
        {
            var game = HttpContext.Session.GetObjectFromJson<Game>("Game");
            if (game == null)
            {
                game = StartNewGame();
            }
            else
            {
                int?[,] tempState = new int?[game.State.GetLength(0), game.State.GetLength(1)];
                int tempScore = game.Score;
                Array.Copy(game.State, tempState, game.State.Length);
                game = CalculateGameState(game, move);
                if (!AreGameStatesEqual(game.State, tempState))
                {
                    game.State = AddNewNumberToGrid(game.State);
                    game.Undo = true;
                    Array.Copy(tempState, game.PreviousState, game.State.Length);
                    game.PreviousScore = tempScore;
                }
                if (NoMoreMovesLeft(game.State))
                    game.GameOver = true;
                else
                    game.GameOver = false;
            }

            var model = new GameViewModel()
            {
                Game = game
            };

            HttpContext.Session.SetObjectAsJson("Game", game);

            return PartialView("_GameCube", model);
        }

        public IActionResult NextMoveVue(string move)
        {
            var game = HttpContext.Session.GetObjectFromJson<Game>("Game");
            if (game == null)
            {
                game = StartNewGame();
            }
            else
            {
                int?[,] tempState = new int?[game.State.GetLength(0), game.State.GetLength(1)];
                int tempScore = game.Score;
                Array.Copy(game.State, tempState, game.State.Length);
                game = CalculateGameState(game, move);
                if (!AreGameStatesEqual(game.State, tempState))
                {
                    game.State = AddNewNumberToGrid(game.State);
                    game.Undo = true;
                    Array.Copy(tempState, game.PreviousState, game.State.Length);
                    game.PreviousScore = tempScore;
                }
                if (NoMoreMovesLeft(game.State))
                    game.GameOver = true;
                else
                    game.GameOver = false;
            }

            var model = new GameViewModel()
            {
                Game = game
            };

            HttpContext.Session.SetObjectAsJson("Game", game);

            return Json(model);
        }


        public IActionResult NewGame()
        {
            Session.Clear();

            Game game = StartNewGame();

            var model = new GameViewModel()
            {
                Game = game
            };

            HttpContext.Session.SetObjectAsJson("Game", game);

            return PartialView("_GameCube", model);
        }

        public IActionResult NewGameVue()
        {
            Session.Clear();

            Game game = StartNewGame();

            var model = new GameViewModel()
            {
                Game = game
            };

            HttpContext.Session.SetObjectAsJson("Game", game);

            return Json(model);
        }

        public IActionResult Undo()
        {
            Game game = HttpContext.Session.GetObjectFromJson<Game>("Game");

            if (game != null)
            {
                game.State = game.PreviousState;
                game.Score = game.PreviousScore;
                game.Undo = false;
                game.GameOver = false;
            }

            var model = new GameViewModel()
            {
                Game = game
            };

            HttpContext.Session.SetObjectAsJson("Game", game);

            return PartialView("_GameCube", model);
        }

        public IActionResult UndoVue()
        {
            Game game = HttpContext.Session.GetObjectFromJson<Game>("Game");

            if (game != null)
            {
                game.State = game.PreviousState;
                game.Score = game.PreviousScore;
                game.Undo = false;
                game.GameOver = false;
            }

            var model = new GameViewModel()
            {
                Game = game
            };

            HttpContext.Session.SetObjectAsJson("Game", game);

            return Json(model);
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
                                if (game.State[x, y] != null && game.State[i, y] != null)
                                {
                                    if (game.State[x, y] == game.State[i, y])
                                    {
                                        game.State[x, y] = (game.State[x, y] ?? 0) + (game.State[i, y] ?? 0);
                                        game.State[i, y] = null;
                                        //optellen bij de score
                                        game.Score += (int)game.State[x, y];
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

                    for (int y = 0; y < game.State.GetLength(1); y++) //kolommen : van 0 tot 3
                    {
                        //deze lus zal de getallen verdubbelen indien ze gelijk zijn
                        for (int x = game.State.GetLength(0) - 1; x >= 1; x--) //rijen : van 3 tot 1
                        {
                            int i = x - 1;

                            while (true)
                            {
                                if (game.State[x, y] != null && game.State[i, y] != null)
                                {
                                    if (game.State[x, y] == game.State[i, y])
                                    {
                                        game.State[x, y] = (game.State[x, y] ?? 0) + (game.State[i, y] ?? 0);
                                        game.State[i, y] = null;
                                        //optellen bij de score
                                        game.Score += (int)game.State[x, y];
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
                        for (int x = game.State.GetLength(0) - 1; x >= 1; x--) // van 3 tot 1
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
                                if (game.State[x, y] != null && game.State[x, i] != null)
                                {
                                    if (game.State[x, y] == game.State[x, i])
                                    {
                                        game.State[x, y] = (game.State[x, y] ?? 0) + (game.State[x, i] ?? 0);
                                        game.State[x, i] = null;
                                        //optellen bij de score
                                        game.Score += (int)game.State[x, y];
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
                                if (game.State[x, y] != null && game.State[x, i] != null)
                                {
                                    if (game.State[x, y] == game.State[x, i])
                                    {
                                        game.State[x, y] = (game.State[x, y] ?? 0) + (game.State[x, i] ?? 0);
                                        game.State[x, i] = null;
                                        //optellen bij de score
                                        game.Score += (int)game.State[x, y];
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

            if (game.Score > game.Highscore)
            {
                game.Highscore = game.Score;
                HttpContext.Response.Cookies.Append("highscore", game.Highscore.ToString());
            }

            return game;
        }

        public int?[,] AddNewNumberToGrid(int?[,] gameState)
        {
            List<Tuple<int, int>> emptyPlaces = new List<Tuple<int, int>>();

            //calculate all empty places in grid
            for (int x = 0; x < gameState.GetLength(0); x++) //rijen
            {
                for (int y = 0; y < gameState.GetLength(1); y++) //kolommen
                {
                    if (gameState[x, y] == null)
                    {
                        emptyPlaces.Add(new Tuple<int, int>(x, y));
                    }
                }
            }

            if (emptyPlaces.Count > 0)
            {
                Random random = new Random();
                Tuple<int, int> emptyPlace = emptyPlaces.ElementAt(random.Next(0, emptyPlaces.Count - 1));

                if (random.Next(0, 9) == 0)
                    gameState[emptyPlace.Item1, emptyPlace.Item2] = 4;
                else
                    gameState[emptyPlace.Item1, emptyPlace.Item2] = 2;
            }

            return gameState;
        }

        private Game StartNewGame()
        {
            Game game = new Game()
            {
                State = new int?[4, 4],
                PreviousState = new int?[4, 4],
                Score = 0,
                PreviousScore = 0,
                Undo = false,
                GameOver = false
            };

            var highscore = HttpContext.Request.Cookies["highscore"];
            if (highscore != null)
            {
                game.Highscore = int.Parse(highscore.ToString());
            }

            game.State = AddNewNumberToGrid(game.State);
            game.PreviousState = game.State;
            game.PreviousScore = game.Score;

            return game;
        }

        private bool NoMoreMovesLeft(int?[,] gameState)
        {
            //check if grid has empty fields, if yes, game can't be over yet
            if (HasEmptyValues(gameState))
                return false;
            
            //then check if there are equal values next to eachother in the whole grid
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                for (int y = 0; y < gameState.GetLength(1); y++)
                {
                    if (!((x == 0 && y == 0) || (x == 0 && y == gameState.GetLength(1) - 1) || (x == gameState.GetLength(0) - 1 && y == 0) || (x == gameState.GetLength(0) - 1 && y == gameState.GetLength(1) - 1))) //ignore the 4 corners, there values will get checked by their neighbours
                    {
                        if (x == 0 && y != 0 && y != gameState.GetLength(1) - 1) // upper row, not corners
                        {
                            if (gameState[x, y] == gameState[x + 1, y] || gameState[x, y] == gameState[x, y - 1] || gameState[x, y] == gameState[x, y + 1])
                                return false;
                        }
                        else if (x == gameState.GetLength(0) - 1 && y != 0 && y != gameState.GetLength(1) - 1) // bottom row, not corners
                        {
                            if (gameState[x, y] == gameState[x - 1, y] || gameState[x, y] == gameState[x, y - 1] || gameState[x, y] == gameState[x, y + 1])
                                return false;
                        }
                        else if (x != 0 && x != gameState.GetLength(0) - 1 && y == 0) // left column, not corners
                        {
                            if (gameState[x, y] == gameState[x - 1, y] || gameState[x, y] == gameState[x + 1, y] || gameState[x, y] == gameState[x, y + 1])
                                return false;
                        }
                        else if (x != 0 && x != gameState.GetLength(0) - 1 && y == gameState.GetLength(1) - 1) // right column, not corners
                        {
                            if (gameState[x, y] == gameState[x - 1, y] || gameState[x, y] == gameState[x + 1, y] || gameState[x, y] == gameState[x, y - 1])
                                return false;
                        }
                        else // (x != 0 && x != gameState.GetLength(0) - 1 && y != 0 && y != gameState.GetLength(1) - 1) //the other fields in the grid
                        {
                            if (gameState[x, y] == gameState[x - 1, y] || gameState[x, y] == gameState[x + 1, y] || gameState[x, y] == gameState[x, y - 1] || gameState[x, y] == gameState[x, y + 1])
                                return false;
                        }
                    }
                }
            }

            //there are no more moves left if you get to here
            return true;
        }

        private bool AreGameStatesEqual(int?[,] gameState1, int?[,] gameState2)
        {
            for (int x = 0; x < gameState1.GetLength(0); x++)
            {
                for (int y = 0; y < gameState1.GetLength(1); y++)
                {
                    if (gameState1[x, y] != gameState2[x, y])
                        return false;
                }
            }
            return true;
        }

        private bool HasEmptyValues(int?[,] gameState)
        {
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                for (int y = 0; y < gameState.GetLength(1); y++)
                {
                    if (gameState[x, y] == null)
                        return true;
                }
            }
            return false;
        }

    }
}
