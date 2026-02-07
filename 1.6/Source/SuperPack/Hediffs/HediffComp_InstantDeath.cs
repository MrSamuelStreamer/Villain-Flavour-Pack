using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SuperPack.Hediffs;

public class HediffComp_InstantDeath : HediffComp
{
    private bool enabled = true;
    private int ticksUntilRecharged;

    public HediffCompProperties_InstantDeath Props => (HediffCompProperties_InstantDeath)props;

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    public bool IsReady => ticksUntilRecharged <= 0;

    public void ConsumeCharge()
    {
        ticksUntilRecharged = Props.rechargeTicks;
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref enabled, "instantDeathEnabled", true);
        Scribe_Values.Look(ref ticksUntilRecharged, "ticksUntilRecharged", 0);
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        if (ticksUntilRecharged > 0)
            ticksUntilRecharged--;
    }

    private string GetGizmoDesc()
    {
        var baseDesc = "SuperPack_InstantDeath_GizmoDesc".Translate();
        if (ticksUntilRecharged <= 0)
            return baseDesc + "\n\n" + "SuperPack_InstantDeath_Ready".Translate();
        return baseDesc + "\n\n" + "SuperPack_InstantDeath_RechargeIn".Translate(ticksUntilRecharged.ToStringTicksToPeriod());
    }

    public IEnumerable<Gizmo> GetGizmos()
    {
        var icon = ContentFinder<UnityEngine.Texture2D>.Get("UI/Icons/Genes/Gene_instant_death", false) ?? BaseContent.BadTex;
        yield return new Command_Toggle
        {
            defaultLabel = "SuperPack_InstantDeath_Gizmo".Translate(),
            defaultDesc = GetGizmoDesc(),
            isActive = () => enabled,
            toggleAction = () => enabled = !enabled,
            icon = icon,
            Disabled = !IsReady,
            disabledReason = "SuperPack_InstantDeath_RechargeIn".Translate(ticksUntilRecharged.ToStringTicksToPeriod()),
        };
    }
}
