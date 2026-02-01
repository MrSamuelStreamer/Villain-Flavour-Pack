using System.Text;
using SuperPack.HarmonyPatches;
using Verse;

namespace SuperPack.Hediffs;

public class HediffComp_TypeWeakness: HediffComp, IPreApplyDamage
{
    private Mote moteAbsorbed;
    public HediffCompProperties_TypeWeakness Props => (HediffCompProperties_TypeWeakness)props;

    public void PreApplyDamage(ref DamageInfo dinfo, ref bool absorbed)
    {
        if (!Props.weaknessTypes.Contains(dinfo.Def.armorCategory)){
            absorbed = true;
            if (moteAbsorbed == null || moteAbsorbed.Destroyed) moteAbsorbed = Pawn_Patch.MakeAbsorbedOverlay(Pawn);
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        moteAbsorbed?.Maintain();
    }

    public override void CompPostMake()
    {
        RegisterSelf();
    }

    public void RegisterSelf()
    {
        Pawn_Patch.preApplyDamageHediffs.Add(this);
    }

    public override string CompDescriptionExtra
    {
        get
        {
            StringBuilder builder = new();
            builder.AppendLine("Immune to:");
            foreach (DamageArmorCategoryDef damageArmorCategoryDef in Props.weaknessTypes)
            {
                builder.AppendLine($" - {damageArmorCategoryDef.LabelCap}");
            }
            return builder.ToString();
        }
    }
}
