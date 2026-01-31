using RimWorld;
using Verse;

namespace SuperPack;

[DefOf]
public static class SuperPackDefOf
{
    // Remember to annotate any Defs that require a DLC as needed e.g.
    // [MayRequireBiotech]
    // public static GeneDef YourPrefix_YourGeneDefName;

    public static SoundDef RMP_GachaGun_Fire;

    static SuperPackDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SuperPackDefOf));
}
