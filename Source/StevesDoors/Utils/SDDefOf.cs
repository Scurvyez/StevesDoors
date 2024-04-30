using RimWorld;
using Verse;

namespace StevesDoors
{
    [DefOf]
    public static class SDDefOf
    {
        public static ThingDef SD_LaserDoorDefault;
        public static ThingDef SD_LaserDoorDouble;

        static SDDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SDDefOf));
        }
    }
}
