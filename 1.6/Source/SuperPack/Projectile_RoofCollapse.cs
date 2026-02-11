using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SuperPack;

public class Projectile_RoofCollapse: Projectile
{

    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        if(Map.roofGrid.Roofed(hitThing.Position)) return;

        List<IntVec3> cells = GenRadial.RadialCellsAround(hitThing.Position, 4, true).Where(c => c.InBounds(Map)).ToList();

        foreach (IntVec3 cell in cells)
        {
            if (!Map.roofGrid.Roofed(cell))
            {
                Map.roofGrid.SetRoof(cell, RoofDefOf.RoofConstructed);
            }
        }

        Map.roofGrid.RoofGridUpdate();

        RoofCollapserImmediate.DropRoofInCells(cells, Map);
    }
}
