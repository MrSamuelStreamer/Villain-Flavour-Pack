using Verse;

namespace SuperPack.Hediffs;

public class HediffCompProperties_Shielded : HediffCompProperties
{
    public ThingDef shieldBuildingDef;
    public IntRange shieldBuildingCount = new(3,3);

    public HediffCompProperties_Shielded()
    {
        compClass = typeof(HediffComp_Shielded);
    }
}
