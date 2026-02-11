using UnityEngine;
using Verse;

namespace SuperPack;

public class Settings : ModSettings
{
    //Use Mod.settings.setting to refer to this setting.
    public bool headshotSound = true;

    public void DoWindowContents(Rect wrect)
    {
        Listing_Standard options = new();
        options.Begin(wrect);

        options.CheckboxLabeled("SuperPack_Settings_Headshot".Translate(), ref headshotSound);
        options.Gap();

        options.End();
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref headshotSound, "headshotSound", true);
    }
}
