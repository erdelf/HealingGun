using System.Linq;

namespace Verse
{
    public class DamageWorker_Heal : DamageWorker
	{
		public override float Apply(DamageInfo dinfo, Thing thing)
		{
			Pawn pawn = thing as Pawn;
            if (pawn != null && pawn.Faction == RimWorld.Faction.OfPlayer)
            {
            	Log.Message("Starting healing on " + pawn.NameStringShort);
            	//Log.Message("Analyzing Pawn");
                Log.Message("Injury Count: " + pawn.health.hediffSet.GetHediffs<Hediff_Injury>().Count());
                /*
                foreach (Hediff_Injury current in pawn.health.hediffSet.GetHediffs<Hediff_Injury>())
                {
                    if ((current.IsNaturallyHealing() || current.NotNaturallyHealingBecauseNeedsTending()) && !current.IsOld()) // basically check for scars and old wounds
                    {
                        Log.Message("Healing " + current.def.defName + " on " + current.Part.def.defName);

                        pawn.health.HealHediff(current, (int)current.Severity + 1);
                    } else
                    {
                        Log.Message("Scars or old wounds can't be healed");
                    }
                }*/

                int maxInjuries = 6;
                int maxInjuriesPerBodypart;

                foreach (BodyPartRecord rec in pawn.health.hediffSet.GetInjuredParts())
                {
                    if (maxInjuries > 0)
                    {
                        maxInjuriesPerBodypart = 2;
                        foreach (Hediff_Injury current in from injury in pawn.health.hediffSet.GetHediffs<Hediff_Injury>() where injury.Part == rec select injury)
                        {
                            if (maxInjuriesPerBodypart > 0)
                            {
                                if (current.CanHealNaturally() && !current.IsOld()) // basically check for scars and old wounds
                                {
                                    Log.Message("Healing " + current.def.defName + " on " + current.Part.def.defName);
                                    current.Heal((int)current.Severity + 1);
                                    maxInjuries--;
                                    maxInjuriesPerBodypart--;
                                }
                                else
                                {
                                    Log.Message("Scars or old wounds can't be healed");
                                }
                            }
                        }
                    }
                }


                Log.Message("Finished healing on " + pawn.NameStringShort);
            }
            return 0f;
		}
	}
}