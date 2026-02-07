using RimWorld;
using Verse;

namespace SuperPack;

[DefOf]
public static class SuperPackDefOf
{
    [MayRequireBiotech]
    public static GeneDef SuperPack_Gene_Walking;

    public static SoundDef RMP_GachaGun_Fire;
    public static ThingDef SuperPack_Mote_Absorbed;

    static SuperPackDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SuperPackDefOf));
}
