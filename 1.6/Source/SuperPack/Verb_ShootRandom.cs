using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace SuperPack;

public class Verb_ShootRandom: Verb_Shoot
{
    public static List<ThingDef> cachedProjectiles = [];

    public override ThingDef Projectile
    {
        get
        {
            if (cachedProjectiles.NullOrEmpty())
            {
                cachedProjectiles = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(def => def.projectile != null);
            }

            return cachedProjectiles.RandomElement();
        }
    }

    protected override bool TryCastShot()
    {
        if (!base.TryCastShot())
        {
            return false;
        }

        SuperPackDefOf.RMP_GachaGun_Fire.PlayOneShot(new TargetInfo(Caster.Position, Caster.Map));
        return true;

    }

}
