using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace SuperPack.HarmonyPatches;

[HarmonyPatch(typeof(Need_Rest))]
[HarmonyPatch(nameof(Need.NeedInterval))]
public static class Need_Rest_Patch
{
    private static readonly Dictionary<Need_Rest, float> SavedRestLevel = new();
    private static readonly FieldInfo NeedPawnField = AccessTools.Field(typeof(Need), "pawn");

    [HarmonyPrefix]
    public static void Prefix(Need_Rest __instance)
    {
        Pawn pawn = (Pawn)NeedPawnField.GetValue(__instance);
        if (pawn?.genes == null || !pawn.genes.HasActiveGene(SuperPackDefOf.SuperPack_Gene_Walking))
            return;
        if (!pawn.IsCaravanMember())
            return;
        SavedRestLevel[__instance] = __instance.CurLevel;
    }

    [HarmonyPostfix]
    public static void Postfix(Need_Rest __instance)
    {
        if (!SavedRestLevel.TryGetValue(__instance, out float saved))
            return;
        SavedRestLevel.Remove(__instance);
        // Only cancel rest loss, allow rest gain when caravan stops to rest
        if (__instance.CurLevel < saved)
            __instance.CurLevel = saved;
    }
}
