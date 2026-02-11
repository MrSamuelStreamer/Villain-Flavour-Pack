using RimWorld;
using RimWorld.Planet;
using Verse;

namespace SuperPack;

public class Projectile_SpawnsPawns: Projectile_SpawnsPawnZeroAge
{
    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        Map map = Map;
        int count = 1;
        ProjectileModExtension modExt = def.GetModExtension<ProjectileModExtension>();
        if (modExt != null)
        {
            count = modExt.spawnCount.RandomInRange;
        }

        for (int i = 0; i < count; i++)
        {
            base.Impact(hitThing, blockedByShield);
            IntVec3 loc = Position;
            if (def.projectile.tryAdjacentFreeSpaces && Position.GetFirstBuilding(map) != null)
            {
                foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(Position))
                {
                    if (c.GetFirstBuilding(map) == null && c.Standable(map))
                    {
                        loc = c;
                        break;
                    }
                }
            }
            PawnKindDef spawnsPawnKind = def.projectile.spawnsPawnKind;
            Faction faction = Launcher.Faction;
            PlanetTile? tile = new();
            Pawn pawn = (Pawn)GenSpawn.Spawn(
                PawnGenerator.GeneratePawn(
                    new PawnGenerationRequest(
                        spawnsPawnKind,
                        faction,
                        tile: tile
                        )
                    ),
                loc,
                map);
            if ((modExt?.angry ?? false) && hitThing is Pawn p)
            {
                pawn.MentalState.ForceHostileTo(p);
            }
        }
    }
}
