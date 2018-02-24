using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace HealingGun
{
    [StaticConstructorOnStartup]
    static class TargetChanger
    {
        static TargetChanger()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.healing_gun");
            harmony.Patch(typeof(AttackTargetFinder).GetMethod("BestAttackTarget"), null, new HarmonyMethod(typeof(TargetChanger).GetMethod("TargetFinderPostfix")));
        }


        public static void TargetFinderPostfix(ref Thing __result, Thing searcher, float maxDist)
        {
            Pawn pawn = searcher as Pawn;
            if (pawn?.equipment?.Primary?.def?.Equals(ThingDef.Named("Healing_Gun")) ?? false)
            {
                IEnumerable<Pawn> pawns = searcher.Map.mapPawns.AllPawnsSpawned.Where((Pawn p) =>
                                               !p.Dead && !p.Faction.HostileTo(pawn.Faction) && p != searcher && GenSight.LineOfSight(searcher.Position, p.Position, searcher.Map) && p.health.hediffSet.GetHediffs<Hediff_Injury>().Any(h=> h.CanHealNaturally() && !h.IsOld()));

                __result = GenClosest.ClosestThing_Global(searcher.Position, pawns.Where(p => p.RaceProps.Humanlike && p.Faction == searcher.Faction), maxDist);
                if(__result==null)
                    __result = GenClosest.ClosestThing_Global(searcher.Position, pawns.Where(p => p.Faction == searcher.Faction), maxDist);
                if (__result==null)
                __result = GenClosest.ClosestThing_Global(searcher.Position, pawns.Where(p => p.RaceProps.Humanlike), maxDist);
                if(__result==null)
                    __result = GenClosest.ClosestThing_Global(searcher.Position, pawns, maxDist);
            }
        }
    }
}
