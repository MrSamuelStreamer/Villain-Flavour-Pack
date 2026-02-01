using Verse;

namespace SuperPack.Hediffs;

public interface IPreApplyDamage
{
    public void PreApplyDamage(ref DamageInfo dinfo, ref bool absorbed);
    public Pawn Pawn { get; }
    public void RegisterSelf();
}
