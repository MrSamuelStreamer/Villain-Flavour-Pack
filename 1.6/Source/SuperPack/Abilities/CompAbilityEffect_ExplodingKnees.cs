using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SuperPack;

public class CompAbilityEffect_ExplodingKnees : CompAbilityEffect
{
    private const float ExplosionRadius = 4f;
    private const int ExplosionDamage = 125; 

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        var pawn = parent.pawn;
        if (pawn?.Map == null)
            return;

        var leg = GetRemainingLeg(pawn);
        if (leg == null)
            return;

        var ignoredThings = new List<Thing> { pawn };
        var explosionSound = DefDatabase<SoundDef>.GetNamedSilentFail("Explosion_Bomb");

        GenExplosion.DoExplosion(
            pawn.Position,
            pawn.Map,
            ExplosionRadius,
            DamageDefOf.Bomb,
            pawn,
            ExplosionDamage,
            armorPenetration: 0.5f,
            explosionSound: explosionSound,
            ignoredThings: ignoredThings
        );

        var missingPart = HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, pawn, leg);
        pawn.health.AddHediff(missingPart);
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        var pawn = parent.pawn;
        if (pawn == null || pawn.Map == null)
            return false;

        if (GetRemainingLeg(pawn) == null)
        {
            if (throwMessages)
            {
                Messages.Message("Cannot use Exploding Knees: no legs remaining.", pawn, MessageTypeDefOf.RejectInput, historical: false);
            }

            return false;
        }

        return base.Valid(target, throwMessages);
    }

    private static BodyPartRecord GetRemainingLeg(Pawn pawn)
    {
        return pawn.health.hediffSet
            .GetNotMissingParts()
            .FirstOrDefault(part => part.def == BodyPartDefOf.Leg);
    }
}

