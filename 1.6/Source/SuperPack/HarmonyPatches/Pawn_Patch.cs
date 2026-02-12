using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using SuperPack.Hediffs;
using Verse;
using Verse.Sound;

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

    [HarmonyPatch(nameof(Pawn.PostApplyDamage))]
    [HarmonyPostfix]
    public static void PostApplyDamage_Patch(Pawn __instance, DamageInfo dinfo)
    {
        if (__instance == null) return;
        if (SuperPack.settings == null || !SuperPack.settings.headshotSound) return;
        if (dinfo.HitPart?.def?.defName == null) return;
        if (!dinfo.HitPart.def.defName.ToLower().Contains("head")) return;
        if (__instance.Map == null) return;

        SuperPackDefOf.RMP_GruntBirthday.PlayOneShot(new TargetInfo(__instance.Position, __instance.Map, false));
        Messages.Message("Boom! Headshot!", new LookTargets(__instance), MessageTypeDefOf.NeutralEvent);
    }
}
