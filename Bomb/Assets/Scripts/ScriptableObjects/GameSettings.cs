using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "MyData", menuName = "Custom/GameSettings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        public int maxPlayers = 12;
        public List<string> devPlayerNames = new() { "Igor", "Герман" };
        public List<Color> colors = new () {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            Color.white,
            Color.black,
            Color.gray,
            Color.brown,
            Color.blueViolet,
            Color.aquamarine,
        };
    }
}