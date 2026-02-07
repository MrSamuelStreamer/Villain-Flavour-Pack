using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SuperPack.HarmonyPatches;

public static class PersonaWeaponAura_Patch
{
    private static readonly HashSet<Thing> AuraWeapons = new();

    private static readonly System.Reflection.FieldInfo CompsField = AccessTools.Field(typeof(ThingWithComps), "comps");

    private static readonly System.Reflection.FieldInfo CompsByTypeField = AccessTools.Field(typeof(ThingWithComps), "compsByType");

    private static readonly System.Reflection.FieldInfo BladelinkTraitsField = AccessTools.Field(typeof(CompBladelinkWeapon), "traits");

    public static bool PawnHasPersonaAura(Pawn pawn)
    {
        if (pawn?.health?.hediffSet == null)
            return false;
        var def = DefDatabase<HediffDef>.GetNamedSilentFail("SuperPack_PersonaWeaponAura");
        return def != null && pawn.health.hediffSet.HasHediff(def);
    }

    private static void AddCompToThing(ThingWithComps thing, ThingComp comp, CompProperties props)
    {
        comp.parent = thing;
        comp.Initialize(props);
        comp.PostPostMake();

        var comps = (List<ThingComp>)CompsField.GetValue(thing);
        if (comps == null)
        {
            comps = new List<ThingComp>();
            CompsField.SetValue(thing, comps);
        }
        comps.Add(comp);
        RebuildCompsByType(thing);
    }

    private static void RemoveCompFromThing(ThingWithComps thing, ThingComp comp)
    {
        var comps = (List<ThingComp>)CompsField.GetValue(thing);
        if (comps == null)
            return;
        comps.Remove(comp);
        RebuildCompsByType(thing);
    }

    private static void RebuildCompsByType(ThingWithComps thing)
    {
        var comps = (List<ThingComp>)CompsField.GetValue(thing);
        if (comps == null || comps.Count == 0)
        {
            CompsByTypeField.SetValue(thing, null);
            return;
        }
        var byType = comps.GroupBy(c => c.GetType()).ToDictionary(g => g.Key, g => g.ToArray());
        CompsByTypeField.SetValue(thing, byType);
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.Notify_EquipmentAdded))]
    public static class Notify_EquipmentAdded_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (eq == null || eq.def?.equipmentType != EquipmentType.Primary)
                return;
            var pawn = __instance.pawn;
            if (pawn == null || !PawnHasPersonaAura(pawn))
                return;
            if (eq.TryGetComp<CompBladelinkWeapon>() != null)
                return;

            var comp = (CompBladelinkWeapon)Activator.CreateInstance(typeof(CompBladelinkWeapon));
            AddCompToThing(eq, comp, new CompProperties_BladelinkWeapon());

            var neverBond = DefDatabase<WeaponTraitDef>.GetNamedSilentFail("NeverBond");
            if (neverBond != null)
            {
                var traits = (List<WeaponTraitDef>)BladelinkTraitsField.GetValue(comp);
                if (traits != null && !traits.Contains(neverBond))
                    traits.Add(neverBond);
            }
            AuraWeapons.Add(eq);
        }
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.Notify_EquipmentRemoved))]
    public static class Notify_EquipmentRemoved_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (eq == null)
                return;
            if (!AuraWeapons.Remove(eq))
                return;

            var bladelink = eq.TryGetComp<CompBladelinkWeapon>();
            if (bladelink == null)
                return;

            bladelink.UnCode();
            RemoveCompFromThing(eq, bladelink);
        }
    }
}
