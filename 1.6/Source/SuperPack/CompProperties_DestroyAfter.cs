using Verse;

namespace SuperPack;

public class CompProperties_DestroyAfter: CompProperties
{
    public IntRange ticks = new (300, 600);
    public bool explodeOnDestroy = false;

    public CompProperties_DestroyAfter()
    {
        compClass = typeof(CompDestroyAfter);
    }
}
