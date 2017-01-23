﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace The2048Game.Models
{
    public class Game
    {
        public int?[,] State { get; set; }
        public bool[,] DoubleUpState { get; set; }
        public int Score { get; set; }
        public int Highscore { get; set; }
    }
}