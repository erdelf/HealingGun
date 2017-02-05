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
            //harmony.PatchAll(Assembly.GetExecutingAssembly());
            harmony.Patch(typeof(AttackTargetFinder).GetMethod("BestAttackTarget"), null, new HarmonyMethod(typeof(TargetChanger).GetMethod("TargetFinderPostfix")));
            /*
            harmony.Patch(typeof(FloatMenuMakerMap).GetMethod("AddHumanlikeOrders", BindingFlags.NonPublic | BindingFlags.Static), null,
                new HarmonyMethod(typeof(TargetChanger).GetMethod("IncapableOfViolenceEquipFix")));

            harmony.Patch(typeof(VerbTracker).GetMethod("GetVerbsCommands"), null, new HarmonyMethod(typeof(TargetChanger).GetMethod("IncapableOfViolenceShootFix")));

            harmony.Patch(AccessTools.Method(typeof(Pawn_StoryTracker), "WorkTagIsDisabled"), null, new HarmonyMethod(AccessTools.Method(typeof(TargetChanger), "FixWorkTagIsDisabled")));
            */
        }
        /*
        public static void FixWorkTagIsDisabled(Pawn_StoryTracker __instance, ref bool __result, WorkTags w)
        {
            __result = __result && (!((Pawn)AccessTools.Field(typeof(Pawn_StoryTracker), "pawn")?.GetValue(__instance))?.equipment?.Primary?.def?.defName?.EqualsIgnoreCase("Healing_Gun") ?? true);
        }

        public static void IncapableOfViolenceShootFix(this VerbTracker __instance, ref IEnumerable<Command> __result)
        {
            if ((__instance.directOwner as ThingComp).parent.def.defName.EqualsIgnoreCase("Healing_Gun"))
            {
                List<Command> commands = new List<Command>();
                foreach (Command c in __result)
                    if (!c.disabled)
                        commands.Add(c);
                    else
                    {
                        c.disabled = false;
                        commands.Add(c);
                    }
                __result = commands;
            }
        }*/

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
        /*
        public static void IncapableOfViolenceEquipFix(ref List<FloatMenuOption> opts, Pawn pawn, Vector3 clickPos)
        {
            FloatMenuOption option = opts.Where(fmo => fmo.Label.Contains(ThingDef.Named("Healing_Gun").label) &&
                fmo.Label.Contains(" (" + "IsIncapableOfViolenceLower".Translate(new object[] { pawn.LabelShort }) + ")")).FirstOrDefault();

            if (option != null)
            {
                opts.Remove(option);

                if (pawn.equipment != null)
                {
                    IntVec3 c = IntVec3.FromVector3(clickPos);
                    ThingWithComps equipment = null;
                    List<Thing> thingList = c.GetThingList(pawn.Map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        if (thingList[i].TryGetComp<CompEquippable>() != null)
                        {
                            equipment = (ThingWithComps)thingList[i];
                            break;
                        }
                    }
                    if (equipment != null)
                    {
                        string labelShort = equipment.LabelShort;
                        if (!pawn.CanReach(equipment, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
                        {
                            opts.Add(new FloatMenuOption("CannotEquip".Translate(new object[]
                            {
                                labelShort
                            }) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                        else if (!pawn.CanReserve(equipment, 1))
                        {
                            opts.Add(new FloatMenuOption("CannotEquip".Translate(new object[]
                            {
                                labelShort
                            }) + " (" + "ReservedBy".Translate(new object[]
                            {
                                pawn.Map.reservationManager.FirstReserverOf(equipment, pawn.Faction, true).LabelShort
                            }) + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                        else if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                        {
                            opts.Add(new FloatMenuOption("CannotEquip".Translate(new object[]
                            {
                                labelShort
                            }) + " (" + "Incapable".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                        else
                        {
                            string text2 = "Equip".Translate(new object[]
                            {
                                labelShort
                            });
                            if (equipment.def.IsRangedWeapon && pawn.story != null && pawn.story.traits.HasTrait(TraitDefOf.Brawler))
                            {
                                text2 = text2 + " " + "EquipWarningBrawler".Translate();
                            }
                            opts.Add(new FloatMenuOption(text2, delegate
                            {
                                equipment.SetForbidden(false, true);
                                pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.Equip, equipment));
                                MoteMaker.MakeStaticMote(equipment.DrawPos, equipment.Map, ThingDefOf.Mote_FeedbackEquip, 1f);
                                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.EquippingWeapons, KnowledgeAmount.Total);
                            }, MenuOptionPriority.High, null, null, 0f, null, null));
                        }
                    }
                }
            }
        }*/
    }
}
