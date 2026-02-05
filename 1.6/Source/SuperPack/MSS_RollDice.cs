using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace MSS_Roll1d6
{
    public class RollTheDice : Building
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
                yield return g;

            yield return new Command_Action
            {
                defaultLabel = "Roll 1d6",
                defaultDesc = "Roll a standard six-sided dice.",
                icon = ContentFinder<Texture2D>.Get("UI/MSS_1d6", true),
                action = MSS_Roll1d6
            };

            yield return new Command_Action
            {
                defaultLabel = "Roll 1d2",
                defaultDesc = "Rolls a two-sided dice (where did we get this idea from?)",
                icon = ContentFinder<Texture2D>.Get("UI/MSS_1d2", true),
                action = MSS_Roll1d2
            };
        }

        public void MSS_Roll1d6()
        {
            int roll = Rand.RangeInclusive(1, 6);

            Find.LetterStack.ReceiveLetter(
                "Rolling the 1d6 dice",
                $"The dice shows: {roll}",
                LetterDefOf.PositiveEvent,
                new TargetInfo(Position, Map)
            );
        }

        public void MSS_Roll1d2()
        {
            int roll = Rand.RangeInclusive(1, 2);

            Find.LetterStack.ReceiveLetter(
                "Rolling the 1d2 dice",
                $"The dice shows: {roll}",
                LetterDefOf.PositiveEvent,
                new TargetInfo(Position, Map)
            );
        }
    }
}