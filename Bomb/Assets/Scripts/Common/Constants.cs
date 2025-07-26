using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Common
{
    public static class Events
    {
        public const string
            EvPlayerAdded = "evPlayerAdded",
            EvPlayerModified = "evPlayerModified",
            EvPlayerRemoved = "evPlayerRemoved",
            EvPlayerSwapped = "evPlayerSwapped",
            EvGameStateChanged = "evGameStateChanged",
            EvCurrentPlayerChanged = "evCurrentPlayerChanged",
            EvTouchNextPlayer = "evTouchNextPlayer",
            EvTouchPrevPlayer = "evTouchPrevPlayer",
            EvTouchStartRound = "evTouchStartRound",
            EvAlert = "evAlert",
            EvCountDownTickChanged = "EvCountDownTickChanged",
            EvPlayerMoveBegin = "EvPlayerMoveBegin",
            EvPlayerMoveEnd = "EvPlayerMoveEnd";

        public static readonly IReadOnlyList<string> ALL = typeof(Events)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => (string)f.GetValue(null))
            .ToList();
    }

    public class Scenes
    {
        public const string
            MainMenu = "MainMenu",
            Game = "Game";
    }
}