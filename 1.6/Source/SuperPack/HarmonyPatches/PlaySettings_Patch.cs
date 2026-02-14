using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SuperPack.HarmonyPatches;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(PlaySettings))]
public static class PlaySettings_Patch
{
    public static Lazy<MethodInfo> _GetMapRect = new(() => AccessTools.Method(typeof(ThingSelectionUtility), "GetMapRect"));
    public static Lazy<MethodInfo> PlaceSpotQualityAt = new(() => AccessTools.Method(typeof(GenPlace), "PlaceSpotQualityAt"));
    public static Lazy<Type> PlaceSpotQuality = new(() => AccessTools.TypeByName("PlaceSpotQuality"));
    public static CellRect GetMapRect(Rect rect)
    {
        return (CellRect) _GetMapRect.Value.Invoke(null, [rect]);
    }


    extension(Map onMap)
    {
        public IntVec3 GetMapCenter()
        {
            return onMap.Center;
        }

        public IntVec3 GetCenterOfScreenOnMap()
        {
            Rect rect = new(0.0f, 0.0f, UI.screenWidth, UI.screenHeight);
            CellRect mapRect = GetMapRect(rect);
            return mapRect.CenterCell;
        }
    }

    public static readonly Texture2D ToggleTex = ContentFinder<Texture2D>.Get(
        "UI/RMP_Dice"
    );

    public static int disableUntil = -1;

    public static HashSet<ThingDef> Things {
        get
        {
            if (field.NullOrEmpty())
            {
                field =
                [
                    SuperPackDefOf.SP_Number_1,
                    SuperPackDefOf.SP_Number_2,
                    SuperPackDefOf.SP_Number_3,
                    SuperPackDefOf.SP_Number_4,
                    SuperPackDefOf.SP_Number_5,
                    SuperPackDefOf.SP_Number_6
                ];
            }
            return field;
        }
    }

    [HarmonyPatch(nameof(PlaySettings.DoPlaySettingsGlobalControls))]
    [HarmonyPostfix]
    public static void DoPlaySettingsGlobalControls_Patch(WidgetRow row)
    {
        if (row.ButtonIcon(ToggleTex, tooltip:"Generate a random number between 1 and 6") && !Things.NullOrEmpty())
        {
            if (disableUntil < Find.TickManager.TicksGame)
            {
                disableUntil = Find.TickManager.TicksGame + 300;
                Map map = Find.CurrentMap;
                ThingDef thingDef = Things.RandomElement();
                Thing thing = ThingMaker.MakeThing(thingDef);
                IntVec3 pos;
                if (TryFindPlaceSpotInRadius(map.GetCenterOfScreenOnMap(), thingDef.defaultPlacingRot, map, thing, 100, false, false, out pos))
                {
                    Skyfaller faller = SkyfallerMaker.MakeSkyfaller(ThingDefOf.ShipChunkIncoming, thing);
                    Thing spawnedFaller = GenSpawn.Spawn(faller, pos, map);

                    Messages.Message("A random number appears!", new LookTargets([thing, faller, spawnedFaller]), MessageTypeDefOf.PositiveEvent);
                }
                else if (TryFindPlaceSpotInRadius(map.GetCenterOfScreenOnMap(), thingDef.defaultPlacingRot, map, thing, 100, false, true, out pos))
                {
                    GenSpawn.Spawn(thing, pos, map);
                    Messages.Message("A random number appears!", new LookTargets(thing), MessageTypeDefOf.PositiveEvent);
                }

            }else{
                Messages.Message($"Enhance your calm! Wait {(disableUntil - Find.TickManager.TicksGame).TicksToSeconds():0.0} seconds", MessageTypeDefOf.RejectInput);
            }
        }
    }

    private static bool TryFindPlaceSpotInRadius(
        IntVec3 center,
        Rot4 rot,
        Map map,
        Thing thing,
        int radius,
        bool allowStacking,
        bool allowRoofed,
        out IntVec3 bestSpot,
        int attempts = 100)
    {
        LocalPlaceSpotQuality placeSpotQuality1 = LocalPlaceSpotQuality.Unusable;
        bestSpot = center;
        while (attempts-- > 0)
        {
            if (!CellFinder.TryRandomClosewalkCellNear(center, map, radius, out IntVec3 result))
            {
                continue;
            }

            bool ExtraValidator(IntVec3 pos)
            {
                if (allowRoofed) return true;
                return !map.roofGrid.Roofed(pos);
            }

            object val = PlaceSpotQualityAt.Value.Invoke(null, [result, rot, map, thing, center, allowStacking, (Predicate<IntVec3>)ExtraValidator ]);
            string name = Enum.GetName(PlaceSpotQuality.Value, val) ?? "Unusable";
            LocalPlaceSpotQuality placeSpotQuality2 = (LocalPlaceSpotQuality)Enum.Parse(typeof(LocalPlaceSpotQuality), name);

            if (placeSpotQuality2 > placeSpotQuality1)
            {
                bestSpot = result;
                placeSpotQuality1 = placeSpotQuality2;
            }
            if (placeSpotQuality1 == LocalPlaceSpotQuality.Perfect)
                break;
        }
        return placeSpotQuality1 > 0;
    }
    private enum LocalPlaceSpotQuality : byte
    {
        Unusable,
        Awful,
        Bad,
        Okay,
        Perfect,
    }
}
