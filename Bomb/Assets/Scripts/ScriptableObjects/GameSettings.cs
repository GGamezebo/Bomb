using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ScriptableObjects
{

    [CreateAssetMenu(fileName = "Data", menuName = "Game/Settings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        public int maxPlayers = 12;
        public List<string> devPlayerNames = new() { "Игорь", "Герман" };

        public float countdownTime = 5.0f;
        public float minBombAliveTime = 10.0f;
        public float maxBombAliveTime = 60.0f;
        public float bonusBombAliveTime = 5;
        public float alertBombTime = 5;
        public float explosionCountdownTime = 5;


        public List<Sprite> colorIcons;


        void OnEnable()
        {
            IEnumerable<int> numbers = Enumerable.Range(0, maxPlayers);
            colorIcons = (from number in numbers select Resources.Load<Sprite>($"ColorIcons/Ellipse {number + 1}"))
                .ToList();
        }
    }
}