using RimWorld;
using Verse;

namespace StevesDoors
{
    [DefOf]
    public static class SDDefOf
    {
        public static ThingDef SD_LaserDoorDefault;

        static SDDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SDDefOf));
        }
    }
}
