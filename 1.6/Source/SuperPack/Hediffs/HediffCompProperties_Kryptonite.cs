using System.Collections.Generic;
using Verse;

namespace SuperPack.Hediffs;

public class HediffCompProperties_Kryptonite: HediffCompProperties
{
    public List<ThingDef> projectileDefs;
    public List<ThingDef> stuffDefs;
    public List<ThingDef> weaponDefs;

    public HediffCompProperties_Kryptonite()
    {
        compClass = typeof(HediffComp_Kryptonite);
    }
}
