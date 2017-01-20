using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using The2048Game.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace The2048Game.Controllers
{
    public class GameController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = new GameViewModel()
            {
                GameState = new int?[4, 4] { { 4096, 2048, 1024, 512 }, { 256, 128, 64, 32 }, { 16, 8, 4, 2 }, { 2, 2, null, null } }
            };

            return View(model);
        }
    }
}
