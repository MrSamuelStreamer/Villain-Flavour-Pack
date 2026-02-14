using System.Collections;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace SuperPack;

[StaticConstructorOnStartup]
public class GlobalDrawLayer_Something : WorldDrawLayerBase
{
    public static readonly Material Sun = MatLoader.LoadMat("World/Sun");

    protected override float Alpha => 0.5f;

    private const float SunDrawSize = 15f;

    protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;

    protected override Quaternion Rotation
    {
        get => Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
    }

    public override IEnumerable Regenerate()
    {
        foreach (object obj in base.Regenerate())
            yield return obj;
        Rand.PushState();
        Rand.Seed = Find.World.info.Seed;
        WorldRendererUtility.PrintQuadTangentialToPlanet(Vector3.forward * 19f, 15f, 0.0f, GetSubMesh(Sun), true);
        Rand.PopState();
        FinalizeMesh(MeshParts.All);
    }
}
