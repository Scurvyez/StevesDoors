using System.Reflection;
using Verse;
using HarmonyLib;

namespace StevesDoors
{
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            var harmony = new Harmony("com.stevesdoors");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //Log.Message("[<color=#668cff>Random Chance</color>]" + "<color=#66ffb3> </color>");
        }
    }
}
