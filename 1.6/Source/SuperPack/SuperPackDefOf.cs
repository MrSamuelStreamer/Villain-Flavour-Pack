using RimWorld;
using Verse;

namespace SuperPack;

[DefOf]
public static class SuperPackDefOf
{
    [MayRequireBiotech]
    public static GeneDef SuperPack_Gene_Walking;

    public static ThingDef RMP_GachaGun;
    public static SoundDef RMP_GachaGun_Fire;
    public static SoundDef RMP_GruntBirthday;
    public static SoundDef RMP_NumberDestroy;
    public static SoundDef RMP_BananaFall;
    public static ThingDef SuperPack_Mote_Absorbed;

    public static ThingDef SP_Number_One;
    public static ThingDef SP_Number_Two;
    public static ThingDef SP_Number_Three;
    public static ThingDef SP_Number_Four;
    public static ThingDef SP_Number_Five;
    public static ThingDef SP_Number_Six;

    static SuperPackDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SuperPackDefOf));
}
