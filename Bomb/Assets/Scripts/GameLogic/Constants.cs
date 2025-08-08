using System;
using System.Collections.Generic;

namespace GameLogic
{
    public enum GameState
    {
        Inactive,
        PlayerChoice,
        Countdown,
        Play,
        Explosion,
        ReadyToStart,
        Result
    }

    public static class GameStateUtils
    {
        public static readonly IReadOnlyList<GameState> ALL = new List<GameState>(
            (GameState[])Enum.GetValues(typeof(GameState))
        );
    }

    public enum WordCondition
    {
        Begin,
        Anywhere,
        End,
    }
}