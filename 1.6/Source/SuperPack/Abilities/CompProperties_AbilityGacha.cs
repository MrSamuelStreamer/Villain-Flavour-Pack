using Verse;

namespace SuperPack;

public class CompProperties_AbilityGacha : RimWorld.CompProperties_AbilityEffect
{
    public CompProperties_AbilityGacha()
    {
        compClass = typeof(CompAbilityEffect_Gacha);
    }
}
