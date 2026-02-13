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
    public static ThingDef SuperPack_Mote_Absorbed;

    public static ThingDef SP_Number_1;
    public static ThingDef SP_Number_2;
    public static ThingDef SP_Number_3;
    public static ThingDef SP_Number_4;
    public static ThingDef SP_Number_5;
    public static ThingDef SP_Number_6;

    static SuperPackDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SuperPackDefOf));
}
