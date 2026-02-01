using System.Linq;
using System.Text;
using RimWorld;
using SuperPack.HarmonyPatches;
using Verse;

namespace SuperPack.Hediffs;

public class HediffComp_Kryptonite: HediffComp, IPreApplyDamage
{
    public HediffCompProperties_Kryptonite Props => (HediffCompProperties_Kryptonite)props;

    private Mote moteAbsorbed;

    public void PreApplyDamage(ref DamageInfo dinfo, ref bool absorbed)
    {
        if(dinfo.Weapon == null) return;
        ThingDef weaponDef = dinfo.Weapon;

        if (weaponDef.Verbs?.Any(x => Props.projectileDefs.Contains(x.defaultProjectile)) ?? false)
        {
            absorbed = false;
        }

        if (Props.weaponDefs.Contains(weaponDef))
        {
            absorbed = false;
            return;
        }

        if (weaponDef.MadeFromStuff && dinfo.Instigator is Pawn instigator)
        {
            ThingWithComps weapon = instigator.equipment.AllEquipmentListForReading.FirstOrDefault(w => w.def == weaponDef);
            if (weapon != null && Props.stuffDefs.Contains(weapon.Stuff))
            {
                absorbed = false;
                return;
            }
        }

        if (moteAbsorbed == null || moteAbsorbed.Destroyed) moteAbsorbed = Pawn_Patch.MakeAbsorbedOverlay(Pawn);

        absorbed = true;
    }

    public override void CompPostMake()
    {
        RegisterSelf();
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        moteAbsorbed?.Maintain();
    }

    public void RegisterSelf()
    {
        Pawn_Patch.preApplyDamageHediffs.Add(this);
    }

    public override string CompDescriptionExtra
    {
        get
        {
            StringBuilder description = new StringBuilder();
            description.AppendLine("\n");

            if (!Props.projectileDefs.NullOrEmpty())
            {
                StringBuilder projectileList = new StringBuilder();
                foreach (ThingDef projectileDef in Props.projectileDefs)
                {
                    projectileList.Append(" - ");
                    projectileList.Append(projectileDef.LabelCap);
                    projectileList.Append("\n");
                }
                description.AppendLine("SuperPack_KryptoniteDescProjectiles".Translate(projectileList.ToString()));
            }

            StringBuilder stuffList = new StringBuilder();
            foreach (ThingDef stuffDef in Props.stuffDefs)
            {
                stuffList.Append(" - ");
                stuffList.Append(stuffDef.LabelCap);
                stuffList.Append("\n");
                description.AppendLine("SuperPack_KryptoniteDescStuff".Translate(stuffList.ToString()));
            }

            StringBuilder weaponList = new StringBuilder();
            foreach (ThingDef weaponDef in Props.weaponDefs)
            {
                weaponList.Append(" - ");
                weaponList.Append(weaponDef.LabelCap);
                weaponList.Append("\n");
                description.AppendLine("SuperPack_KryptoniteDescWeapons".Translate(weaponList.ToString()));
            }

            return description.ToString();
        }
    }
}
