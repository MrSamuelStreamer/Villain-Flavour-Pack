using Verse;

namespace SuperPack.Hediffs;

public class HediffCompProperties_InstantDeath : HediffCompProperties
{
    public int rechargeTicks = 60000;

    public HediffCompProperties_InstantDeath()
    {
        compClass = typeof(HediffComp_InstantDeath);
    }
}
