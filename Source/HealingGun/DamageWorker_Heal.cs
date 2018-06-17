namespace HealingGun
{
    using System.Linq;
    using Verse;

    public class DamageWorker_Heal : DamageWorker
	{
        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
		{
            if (victim is Pawn pawn)
            {
                Log.Message(text: "Starting healing on " + pawn.Name.ToStringShort);
                //Log.Message("Analyzing Pawn");
                Log.Message(text: "Injury Count: " + pawn.health.hediffSet.GetHediffs<Hediff_Injury>().Count());
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

                foreach (BodyPartRecord rec in pawn.health.hediffSet.GetInjuredParts())
                {
                    if (maxInjuries <= 0) continue;
                    int maxInjuriesPerBodypart = 2;
                    foreach (Hediff_Injury current in from injury in pawn.health.hediffSet.GetHediffs<Hediff_Injury>() where injury.Part == rec select injury)
                    {
                        if (maxInjuriesPerBodypart <= 0) continue;
                        if (current.CanHealNaturally() && !current.IsPermanent()) // basically check for scars and old wounds
                        {
                            Log.Message(text: "Healing " + current.def.defName + " on " + current.Part.def.defName);
                            current.Heal(amount: (int)current.Severity                  + 1);
                            maxInjuries--;
                            maxInjuriesPerBodypart--;
                        }
                        else
                        {
                            Log.Message(text: "Scars or old wounds can't be healed");
                        }
                    }
                }


                Log.Message(text: "Finished healing on " + pawn.Name.ToStringShort);
            }
            return new DamageResult();
		}
	}
}