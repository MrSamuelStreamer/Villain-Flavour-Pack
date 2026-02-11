using Verse;

namespace SuperPack;

public class ProjectileModExtension: DefModExtension
{
    public IntRange spawnCount = new(1,1);
    public bool angry = false;
}
