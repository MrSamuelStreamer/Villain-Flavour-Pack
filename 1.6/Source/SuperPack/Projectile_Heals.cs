using RimWorld;
using RimWorld.Planet;
using Verse;

namespace SuperPack;

public class Projectile_Heals: Projectile
{
    public bool CanHit(Pawn p)
    {
        return HealthUtility.TryGetWorstHealthCondition(p, out Hediff _, out BodyPartRecord _);
    }

    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        if (hitThing is Pawn pawn && CanHit(pawn))
        {
            TaggedString details = HealthUtility.FixWorstHealthCondition(pawn);
            Messages.Message(details, pawn, MessageTypeDefOf.NeutralEvent);
        }
    }
}
