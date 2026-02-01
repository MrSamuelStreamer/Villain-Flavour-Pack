using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SuperPack.Hediffs;
using Verse;

namespace SuperPack.HarmonyPatches;

[HarmonyPatch(typeof(Pawn))]
public static class Pawn_Patch
{
    public static HashSet<IPreApplyDamage> preApplyDamageHediffs = new();

    [HarmonyPatch(nameof(Pawn.PreApplyDamage))]
    [HarmonyPostfix]
    public static void PreApplyDamage_Patch(Pawn __instance, ref DamageInfo dinfo, ref bool absorbed)
    {
        bool any = preApplyDamageHediffs.Any(x => x.Pawn == __instance);

        if(!any) return;

        foreach (IPreApplyDamage hediff in preApplyDamageHediffs.Where(x => x.Pawn == __instance))
        {
            hediff.PreApplyDamage(ref dinfo, ref absorbed);
        }
    }

    public static Mote MakeAbsorbedOverlay(Thing stunnedThing)
    {
        Mote newThing = (Mote) ThingMaker.MakeThing(SuperPackDefOf.SuperPack_Mote_Absorbed);
        newThing.Attach((TargetInfo) stunnedThing);
        GenSpawn.Spawn(newThing, stunnedThing.Position, stunnedThing.Map);
        return newThing;
    }
}
