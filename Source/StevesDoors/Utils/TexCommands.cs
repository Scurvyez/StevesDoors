using UnityEngine;
using Verse;

namespace StevesDoors
{
    [StaticConstructorOnStartup]
    public static class TexCommands
    {
        public static readonly Texture2D UnrestrictedAccess = ContentFinder<Texture2D>.Get("StevesDoors/UI/Commands/SD_UnrestrictedAccess");
        public static readonly Texture2D RestrictedAccess = ContentFinder<Texture2D>.Get("StevesDoors/UI/Commands/SD_RestrictedAccess");
        public static readonly Texture2D AllowedAccess = ContentFinder<Texture2D>.Get("StevesDoors/UI/Commands/SD_AllowedAccess");
    }
}
