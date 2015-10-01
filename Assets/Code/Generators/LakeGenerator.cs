﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Code.Layout;
using Assets.Code.Settings;

namespace Assets.Code.Generators
{
    public class LakeGenerator : ZoneGenerator
    {
        public LakeGenerator(Zone zone, Land land, ILandSettings landSettings) : base(zone, land, landSettings)
        {
        }
    }
}
