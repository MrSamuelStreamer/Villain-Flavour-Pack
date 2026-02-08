using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SuperPack;

public class CompAbilityEffect_Gacha : RimWorld.CompAbilityEffect
{
    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        var cell = target.Cell;
        var map = parent.pawn?.Map;
        if (map == null || !cell.IsValid || !cell.InBounds(map) || !cell.Standable(map))
            return;

        var thingDef = GachaSpawnHelper.GetRandomSpawnableThingDef();
        if (thingDef == null)
            return;

        Thing thing = null;
        try
        {
            thing = ThingMaker.MakeThing(thingDef);
        }
        catch
        {
            return;
        }

        if (thing == null)
            return;

        if (thing is Building building)
            building.SetFaction(parent.pawn.Faction);

        GenSpawn.Spawn(thing, cell, map);
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        if (!target.IsValid || !target.Cell.InBounds(parent.pawn.Map) || !target.Cell.Standable(parent.pawn.Map))
            return false;
        return true;
    }
}

public static class GachaSpawnHelper
{
    private static List<ThingDef> _spawnableThingDefsCache;

    public static ThingDef GetRandomSpawnableThingDef()
    {
        var list = GetSpawnableThingDefs();
        return list.Count > 0 ? list.RandomElement() : null;
    }

    public static List<ThingDef> GetSpawnableThingDefs()
    {
        if (_spawnableThingDefsCache != null)
            return _spawnableThingDefsCache;

        var list = new List<ThingDef>();
        foreach (var def in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (def.thingClass == null)
                continue;
            if (def.thingClass == typeof(Corpse))
                continue;
            if (def.IsBlueprint || def.IsFrame)
                continue;
            if (def.category == ThingCategory.Pawn)
                continue;
            if (def.category == ThingCategory.Projectile || def.category == ThingCategory.Mote)
                continue;
            if (def.category == ThingCategory.Ethereal)
                continue;
            if (def.MadeFromStuff)
                continue;
            if (def.category == ThingCategory.Item)
                list.Add(def);
        }

        _spawnableThingDefsCache = list;
        return list;
    }
}
