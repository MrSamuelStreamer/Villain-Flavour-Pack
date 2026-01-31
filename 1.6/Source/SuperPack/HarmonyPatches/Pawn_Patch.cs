using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SuperPack.Hediffs;
using Verse;

namespace SuperPack.HarmonyPatches;

[HarmonyPatch(typeof(Pawn))]
public static class Pawn_Patch
{
    public static HashSet<Pawn> shieldedPawns = new();

    [HarmonyPatch(nameof(Pawn.PreApplyDamage))]
    [HarmonyPostfix]
    public static void PreApplyDamage_Patch(Pawn __instance, ref bool absorbed)
    {
        if(!shieldedPawns.Contains(__instance)) return;

        foreach (HediffComp_Shielded shielded in __instance.health.hediffSet.hediffs.OfType<HediffWithComps>().SelectMany(h=>h.comps.OfType<HediffComp_Shielded>()))
        {
            if(shielded.Shielded) absorbed = true;
        }

    }
}
