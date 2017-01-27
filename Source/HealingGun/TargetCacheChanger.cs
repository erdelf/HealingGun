using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace HealingGun
{
    /*
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(AttackTargetsCache))]
    [HarmonyPatch("GetPotentialTargetsFor")]
    static class TargetCacheChanger
    {
        static TargetCacheChanger()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.erdelf.healing_gun");

            harmony.PatchAll(typeof(TargetCacheChanger).Module);
        }

        static void Postfix(AttackTargetsCache _this, ref List<IAttackTarget> result, ref Thing th)
        {
            Pawn pawn = th as Pawn;
            if(pawn!=null && pawn.equipment.Primary.def.Equals(ThingDef.Named("Healing_Gun")))
            {
                result.Clear();
                result = th.Map.mapPawns.AllPawnsSpawned.Where((Pawn p) => !p.Dead && !p.Faction.HostileTo(pawn.Faction)).Cast<IAttackTarget>().ToList();
            }
        }
    }*/
}
