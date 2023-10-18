using HarmonyLib;
using Verse.AI;
using Verse;

namespace StevesDoors
{
    /*
    [HarmonyPatch(typeof(Pawn_PathFollower))]
    [HarmonyPatch("TryEnterNextPathCell")]
    public static class Pawn_PathFollowerTryEnterNextPathCell_Prefix
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn_PathFollower __instance, Pawn ___pawn, IntVec3 ___nextCell)
        {
            if (___pawn != null && ___pawn.pather != null && ___pawn.pather.Moving && ___nextCell.IsValid && ___nextCell != ___pawn.Position)
            {
                Building_UnmirroredDoor door = ___nextCell.GetEdifice(___pawn.Map) as Building_UnmirroredDoor;

                if (door != null && door.IsAccessDoor && !door.AllowedForFaction(___pawn.Faction))
                {
                    Job job = ___pawn.CurJob;

                    if (job != null)
                    {
                        // Clear the current job
                        //___pawn.jobs.StopAll();
                        //___pawn.pather.StopDead();

                        ___pawn.stances.stunner.StunFor(60, ___pawn, addBattleLog: false, showMote: false);
                        ___pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, startNewJob: false, canReturnToPool: true);
                        ___pawn.jobs.ClearQueuedJobs();
                    }

                    return false; // Prevent entering the next path cell
                }
            }

            return true;
        }
    }
    */
}
