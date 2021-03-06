﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;

namespace ECS_Test.Core
{
    class Colours
    {
        public static RLColor FloorBackground = RLColor.Black;
        public static RLColor Floor = Swatch.AlternateDarkest;
        public static RLColor FloorBackgroundFov = Swatch.DbDark;
        public static RLColor FloorFov = Swatch.Alternate;

        public static RLColor WallBackground = Swatch.SecondaryDarkest;
        public static RLColor Wall = Swatch.Secondary;
        public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
        public static RLColor WallFov = Swatch.SecondaryLighter;

        public static RLColor DoorBackground = Swatch.CompilmentDarkest;
        public static RLColor Door = Swatch.CompilmentLighter;
        public static RLColor DoorBackgroundFov = Swatch.CompilmenteDarker;
        public static RLColor DoorFov = Swatch.CompilmentLightest;


        public static RLColor TextHeading = Swatch.DbLight;

        public static RLColor Player = Swatch.DbLight;
        public static RLColor OrcColour = Swatch.DbBrightWood;
    }
}
