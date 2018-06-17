namespace HealingGun
{
    using System.Collections.Generic;
    using System.Linq;
    using Harmony;
    using RimWorld;
    using Verse;
    using Verse.AI;

    [StaticConstructorOnStartup]
    internal static class TargetChanger
    {
        static TargetChanger()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(id: "rimworld.erdelf.healing_gun");
            harmony.Patch(original: typeof(AttackTargetFinder).GetMethod(name: "BestAttackTarget"), prefix: null, postfix: new HarmonyMethod(method: typeof(TargetChanger).GetMethod(name: nameof(TargetFinderPostfix))));
        }

        public static void TargetFinderPostfix(ref Thing __result, Thing searcher, float maxDist)
        {
            Pawn pawn = searcher as Pawn;
            if (!(pawn?.equipment?.Primary?.def?.Equals(obj: ThingDef.Named(defName: "Healing_Gun")) ?? false)) return;
            IEnumerable<Pawn> pawns = searcher.Map.mapPawns.AllPawnsSpawned.Where(predicate: p =>
                !p.Dead && !p.Faction.HostileTo(other: pawn.Faction) && p != searcher && GenSight.LineOfSight(start: searcher.Position, end: p.Position, map: searcher.Map) &&
                p.health.hediffSet.GetHediffs<Hediff_Injury>().Any(predicate: h => h.CanHealNaturally() && !h.IsPermanent())).ToArray();

            __result =
                ((GenClosest.ClosestThing_Global(center: searcher.Position, searchSet: pawns.Where(predicate: p => p.RaceProps.Humanlike && p.Faction == searcher.Faction), maxDistance: maxDist) ??
                  GenClosest.ClosestThing_Global(center: searcher.Position, searchSet: pawns.Where(predicate: p => p.Faction == searcher.Faction),                          maxDistance: maxDist)) ??
                 GenClosest.ClosestThing_Global(center: searcher.Position, searchSet: pawns.Where(predicate: p => p.RaceProps.Humanlike), maxDistance: maxDist)) ??
                GenClosest.ClosestThing_Global(center: searcher.Position, searchSet: pawns, maxDistance: maxDist);
        }
    }
}
