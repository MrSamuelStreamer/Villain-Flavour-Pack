using RimWorld;
using Verse;

namespace SuperPack;

public class CompDestroyAfter: ThingComp
{
    public CompProperties_DestroyAfter Props => (CompProperties_DestroyAfter)props;

    public int destroyAfter;

    public override void PostPostMake()
    {
        base.PostPostMake();
        destroyAfter = Find.TickManager.TicksGame + Props.ticks.RandomInRange;
    }

    public override void CompTickInterval(int delta)
    {
        base.CompTickInterval(delta);
        if (destroyAfter < Find.TickManager.TicksGame)
        {
            if (Props.explodeOnDestroy)
            {
                GenExplosion.DoExplosion(parent.Position,
                    parent.Map,
                    3,
                    DamageDefOf.Psychic,
                    parent, 1,
                    explosionSound:SuperPackDefOf.RMP_NumberDestroy,
                    doSoundEffects:true,
                    postExplosionGasType:GasType.BlindSmoke);
            }
            parent.Destroy();
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref destroyAfter, "destroyAfter");
    }


    public override string CompInspectStringExtra() => $"Self destructs in {(int)(destroyAfter - Find.TickManager.TicksGame).TicksToSeconds()} seconds.";
}
