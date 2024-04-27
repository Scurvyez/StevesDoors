﻿using UnityEngine;
using Verse;

namespace StevesDoors
{
    [StaticConstructorOnStartup]
    public static class Assets
    {
        public static readonly Texture2D LaserDoorTex = ContentFinder<Texture2D>.Get("StevesDoors/Things/Building/Door/SD_LaserDoorDefault_MenuIcon");
    }
}