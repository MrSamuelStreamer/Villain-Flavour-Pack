using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using SuperPack.Comps;
using Verse;

namespace SuperPack.HarmonyPatches;

[HarmonyPatch]
public static class JobDriver_Reload_Patch
{
    // I have no idea how this works, but it does. Don't ask me questions.
    [HarmonyPatch]
    public static MethodBase TargetMethod()
    {
        Type innerType = AccessTools.Inner(typeof(JobDriver_Reload), "<MakeNewToils>d__5");
        if (innerType == null)
        {
            Log.Error("[SuperPack] Could not find inner type <MakeNewToils>d__5 in JobDriver_Reload");
            return null;
        }
        MethodInfo moveNext = AccessTools.Method(innerType, "MoveNext");
        if (moveNext == null)
        {
            Log.Error("[SuperPack] Could not find MoveNext in " + innerType.FullName);
        }
        return moveNext;
    }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
        List<CodeInstruction> code = new(instructions);

        MethodInfo tryGetCompReloadable = AccessTools.Method(typeof(ThingCompUtility), nameof(ThingCompUtility.TryGetComp), new[] { typeof(Thing) })
            ?.MakeGenericMethod(typeof(CompEquippableReloadable));

        // Find:
        //   call !!0 Verse.ThingCompUtility::TryGetComp<CompEquippableAbilityReloadable>(class Verse.Thing)
        //   stfld class RimWorld.Utility.IReloadableComp RimWorld.JobDriver_Reload/'<>c__DisplayClass5_0'::reloadable
        //
        // Then inject right after:
        //   if (reloadable == null) reloadable = this.<>4__this.Gear.TryGetComp<CompEquippableReloadable>();

        Type innerType = AccessTools.Inner(typeof(JobDriver_Reload), "<MakeNewToils>d__5");
        FieldInfo displayClassInIterator = AccessTools.Field(innerType, "<>8__1")
            ?? AccessTools.Field(innerType, "CS$<>8__locals1");

        FieldInfo jobDriverInIterator = AccessTools.Field(innerType, "<>4__this");

        MethodInfo getGear = AccessTools.Method(typeof(JobDriver_Reload), "get_Gear");

        for (int i = 0; i < code.Count - 1; i++)
        {
            // The IL code uses 'call' for extension methods like TryGetComp
            if ((code[i].opcode == OpCodes.Call || code[i].opcode == OpCodes.Callvirt) &&
                code[i].operand is MethodInfo mi && mi.Name == "TryGetComp" && mi.IsGenericMethod)
            {
                Type[] genericArgs = mi.GetGenericArguments();
                if (genericArgs.Length != 1 || (genericArgs[0].Name != "CompEquippableAbilityReloadable"))
                    continue;

                CodeInstruction stfld = code[i + 1];
                if (stfld.opcode != OpCodes.Stfld || stfld.operand is not FieldInfo fi || fi.Name != "reloadable")
                    continue;

                object reloadableField = stfld.operand;

                if (i + 2 >= code.Count)
                    continue;

                Label continueLabel = il.DefineLabel();
                code[i + 2].labels.Add(continueLabel);

                if (jobDriverInIterator == null || getGear == null)
                {
                    Log.Error("[SuperPack] Failed to resolve critical metadata for JobDriver_Reload_Patch");
                    continue;
                }

                List<CodeInstruction> injected = new()
                {
                    // if (displayClass.reloadable != null) goto continueLabel;
                    // Note: displayClass is already on the stack if we find where it was loaded,
                    // but it's easier to just load it again from the iterator fields if possible.
                    // Or even better, load it from the stfld instruction's context.
                };

                // Find how to load the display class. It was just used for stfld.
                // In MoveNext, it's usually ldarg.0 + ldfld <>8__1

                CodeInstruction loadDisplayClass = null;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (code[j].opcode == OpCodes.Ldfld && (code[j].operand == displayClassInIterator || (code[j].operand is FieldInfo sfi && (sfi.Name == "<>8__1" || sfi.Name == "CS$<>8__locals1"))))
                    {
                        if (j > 0 && code[j - 1].opcode == OpCodes.Ldarg_0)
                        {
                            loadDisplayClass = code[j].Clone();
                            break;
                        }
                    }
                }

                if (loadDisplayClass == null)
                    continue;

                injected.Add(new CodeInstruction(OpCodes.Ldarg_0));
                injected.Add(loadDisplayClass.Clone());
                injected.Add(new CodeInstruction(OpCodes.Ldfld, reloadableField));
                injected.Add(new CodeInstruction(OpCodes.Brtrue, continueLabel));

                // displayClass.reloadable = this.<>4__this.Gear.TryGetComp<CompEquippableReloadable>();
                injected.Add(new CodeInstruction(OpCodes.Ldarg_0));
                injected.Add(loadDisplayClass.Clone());

                injected.Add(new CodeInstruction(OpCodes.Ldarg_0));
                injected.Add(new CodeInstruction(OpCodes.Ldfld, jobDriverInIterator));
                injected.Add(new CodeInstruction(OpCodes.Call, getGear));
                injected.Add(new CodeInstruction(OpCodes.Call, tryGetCompReloadable));
                injected.Add(new CodeInstruction(OpCodes.Stfld, reloadableField));

                code.InsertRange(i + 2, injected);
                break;
            }
        }

        return code;
    }

    private static bool IsStloc(CodeInstruction ins, out object local)
    {
        local = null;

        if (ins.opcode == OpCodes.Stloc_0)
        {
            local = 0;
            return true;
        }

        if (ins.opcode == OpCodes.Stloc_1)
        {
            local = 1;
            return true;
        }

        if (ins.opcode == OpCodes.Stloc_2)
        {
            local = 2;
            return true;
        }

        if (ins.opcode == OpCodes.Stloc_3)
        {
            local = 3;
            return true;
        }

        if (ins.opcode == OpCodes.Stloc_S || ins.opcode == OpCodes.Stloc)
        {
            local = ins.operand; // LocalBuilder or index depending on Harmony version
            return true;
        }

        return false;
    }

    private static CodeInstruction Ldloc(object local)
    {
        // Harmony/IL can represent locals as LocalBuilder or int index depending on context.
        if (local is int idx)
        {
            return idx switch
            {
                0 => new CodeInstruction(OpCodes.Ldloc_0),
                1 => new CodeInstruction(OpCodes.Ldloc_1),
                2 => new CodeInstruction(OpCodes.Ldloc_2),
                3 => new CodeInstruction(OpCodes.Ldloc_3),
                _ => new CodeInstruction(OpCodes.Ldloc, idx),
            };
        }

        return new CodeInstruction(OpCodes.Ldloc, local);
    }

    private static CodeInstruction Stloc(object local)
    {
        if (local is int idx)
        {
            return idx switch
            {
                0 => new CodeInstruction(OpCodes.Stloc_0),
                1 => new CodeInstruction(OpCodes.Stloc_1),
                2 => new CodeInstruction(OpCodes.Stloc_2),
                3 => new CodeInstruction(OpCodes.Stloc_3),
                _ => new CodeInstruction(OpCodes.Stloc, idx),
            };
        }

        return new CodeInstruction(OpCodes.Stloc, local);
    }
}
