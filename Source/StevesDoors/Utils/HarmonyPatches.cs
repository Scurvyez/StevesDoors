using HarmonyLib;
using Verse;

namespace StevesDoors
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new(id: "rimworld.scurvyez.stevesdoors");


        }
    }
}
