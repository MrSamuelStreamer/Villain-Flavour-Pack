using Verse;
using UnityEngine;
using HarmonyLib;

namespace SuperPack;

public class SuperPack : Mod
{
    public static Settings settings;

    public SuperPack(ModContentPack content) : base(content)
    {

        // initialize settings
        settings = GetSettings<Settings>();
#if DEBUG
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("keyz182.rimworld.SuperPack.main");
        harmony.PatchAll();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        settings.DoWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "SuperPack_SettingsCategory".Translate();
    }
}
