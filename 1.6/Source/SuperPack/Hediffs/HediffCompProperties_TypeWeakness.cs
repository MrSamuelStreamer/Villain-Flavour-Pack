using System.Collections.Generic;
using Verse;

namespace SuperPack.Hediffs;

public class HediffCompProperties_TypeWeakness: HediffCompProperties
{
    public List<DamageArmorCategoryDef> weaknessTypes;

    public HediffCompProperties_TypeWeakness()
    {
        compClass = typeof(HediffComp_TypeWeakness);
    }
}
