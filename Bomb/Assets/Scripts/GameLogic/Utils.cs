using System;

namespace GameLogic
{
    public static class Utils
    {
        public static WordCondition GetWordConditionRandom()
        {
            return Utils.Conditions[_rand.Next(Utils.Conditions.Length)];
        }

        private static readonly Random _rand = new Random();
        private static readonly WordCondition[] Conditions =
        {
            WordCondition.Begin, WordCondition.Anywhere, WordCondition.End
        };
    }
}
