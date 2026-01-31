using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SuperPack.HarmonyPatches;
using Verse;

namespace SuperPack.Hediffs;

public class HediffComp_Shielded: HediffComp
{
    public Dictionary<Map, HashSet<Thing>> ShieldBuildings = new();
    public Dictionary<Map, bool> ShieldDestroyed = new();

    public HediffCompProperties_Shielded Props => (HediffCompProperties_Shielded)props;

    public Map Map => parent.pawn.Map;


    public bool Shielded => !ShieldDestroyed[Map];

    public int ShieldBuildingsCount => ShieldBuildings[Map].Count;
    public int ShieldDestroyedCount => ShieldBuildings[Map].Count(bld=>bld.Destroyed);

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (!ShieldBuildings.ContainsKey(Map) && (!ShieldDestroyed.ContainsKey(Map) || !ShieldDestroyed[Map]))
        {
            GenerateShield();
        }

        if (!ShieldDestroyed[Map] && parent.pawn.IsHashIntervalTick(600))
        {
            if (ShieldBuildings[Map].All(bld => bld.Destroyed))
            {
                ShieldDestroyed[Map] = true;
            }
        }
    }

    public override void CompPostMake()
    {
        Pawn_Patch.shieldedPawns.Add(parent.pawn);
    }

    public bool CanPlaceAt(IntVec3 cell)
    {
        if(!cell.Standable(Map)) return false;
        if(Map.thingGrid.CellContains(cell, ThingCategory.Building)) return false;
        if (!cell.Standable(Map)) return false;
        if (cell.Roofed(Map)) return false;

        return true;
    }

    public void GenerateShield()
    {
        ShieldDestroyed.Add(Map, false);

        if(ShieldBuildings.ContainsKey(Map)) return;

        HashSet<Thing> buildings = new();

        int count = Props.shieldBuildingCount.RandomInRange;

        for (int i = 0; i < count; i++)
        {
            Thing building = ThingMaker.MakeThing(Props.shieldBuildingDef);
            building.SetFaction(parent.pawn.Faction);
            if (CellFinderLoose.TryFindRandomNotEdgeCellWith(5, CanPlaceAt, Map, out IntVec3 result))
            {
                GenSpawn.Spawn(building, result, Map);
                buildings.Add(building);
            }
        }

        ShieldBuildings.Add(Map, buildings);

        SendLetter();
    }

    public void SendLetter()
    {
        LookTargets targets = new();
        targets.targets.Add(parent.pawn);

        foreach (Thing thing in ShieldBuildings[Map])
        {
            targets.targets.Add(thing);
        }

        Letter letter = LetterMaker.MakeLetter(
            "SuperPack_ShieldLetterTitle".Translate(parent.pawn.Named("PAWN")),
            "SuperPack_ShieldLetterText".Translate(parent.pawn.Named("PAWN"), Props.shieldBuildingDef.LabelCap),
            LetterDefOf.NegativeEvent,
            targets
        );
        Find.LetterStack.ReceiveLetter(letter);
    }

    public override void Notify_Spawned()
    {
        GenerateShield();
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Collections.Look(ref ShieldBuildings, "ShieldBuildings", LookMode.Reference);
        Scribe_Collections.Look(ref ShieldDestroyed, "ShieldDestroyed", LookMode.Value);
    }


    public override string CompDescriptionExtra
    {
        get
        {
            return Shielded ? "SuperPack_Shielded".Translate(Props.shieldBuildingDef.LabelCap, ShieldBuildingsCount - ShieldDestroyedCount, ShieldBuildingsCount) : "SuperPack_UnShielded".Translate(Props.shieldBuildingDef.LabelCap);
        }
    }
}
