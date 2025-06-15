using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "Game/Settings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        public int maxPlayers = 12;
        public List<string> devPlayerNames = new() { "Игорь", "Герман" };
        public List<Color> colors = new () {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            Color.orange,
            Color.black,
            Color.gray,
            Color.brown,
            Color.blueViolet,
            Color.aquamarine,
        };
        
        public float countdownTime = 5.0f;
        public float minBombAliveTime = 10.0f;
        public float maxBombAliveTime = 60.0f;
        public float bonusBombAliveTime = 5;
        public float alertBombTime = 5;
        public float explosionCountdownTime = 5;
    }
}