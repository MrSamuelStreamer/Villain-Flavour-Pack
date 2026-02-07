using System.Collections.Generic;
using HarmonyLib;
using SuperPack.Hediffs;
using Verse;

namespace SuperPack.HarmonyPatches;

[HarmonyPatch(typeof(Pawn))]
public static class InstantDeath_Patch
{
    private static HediffComp_InstantDeath GetInstantDeathComp(Pawn pawn)
    {
        if (pawn?.health?.hediffSet?.hediffs == null)
            return null;
        foreach (var hediff in pawn.health.hediffSet.hediffs)
        {
            if (hediff is not HediffWithComps hwc)
                continue;
            foreach (var comp in hwc.comps)
            {
                if (comp is HediffComp_InstantDeath instantDeath)
                    return instantDeath;
            }
        }
        return null;
    }

    [HarmonyPatch(nameof(Pawn.PostApplyDamage))]
    [HarmonyPostfix]
    public static void PostApplyDamage_Postfix(Pawn __instance, DamageInfo dinfo, float totalDamageDealt)
    {
        if (totalDamageDealt <= 0f)
            return;
        var comp = GetInstantDeathComp(__instance);
        if (comp == null || !comp.Enabled || !comp.IsReady)
            return;
        comp.ConsumeCharge();

        var instigator = dinfo.Instigator;
        if (instigator == null)
            return;
        if (instigator is Pawn instigatorPawn && !instigatorPawn.Dead)
        {
            instigatorPawn.Kill(null);
            return;
        }
        if (instigator is Building building && building.Spawned)
            building.Destroy(DestroyMode.Vanish);
    }

    [HarmonyPatch(nameof(Pawn.GetGizmos))]
    [HarmonyPostfix]
    public static void GetGizmos_Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
    {
        var comp = GetInstantDeathComp(__instance);
        if (comp == null)
            return;
        var list = new List<Gizmo>(__result);
        foreach (var g in comp.GetGizmos())
            list.Add(g);
        __result = list;
    }
}
