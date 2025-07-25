using System.Collections.Generic;
using System.Linq;
using Account;
using UnityEngine;
using UnityEngine.Serialization;


namespace ScriptableObjects
{

    [CreateAssetMenu(fileName = "Data", menuName = "Game/Settings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        public int maxPlayers = 12;
        public List<PlayerInfo> devPlayerNames = new()
        {
            new PlayerInfo("Igor", 0),
            new PlayerInfo("German", 1),
        };

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

        public string[] cards =
        {
            "лом", "ка", "ев", "ма", "ли", "ук", "за", "вед", "ди", "ло", "аз", "ост",
            "от", "мат", "воз", "лю", "вик", "да", "хоз", "ран", "ат", "те", "ом",
            "ал", "ак", "ов", "изм", "вар", "га", "тор", "ус", "та", "ро", "ад",
            "фон", "лог", "ром", "он", "рез", "уб", "ни", "во", "вод", "акт", "по",
            "ча", "ор", "пан", "ле", "док", "не", "ке", "век", "мер", "мас", "бол",
            "суда", "ок", "па", "коп", "дел", "ил", "со", "на", "мо", "уз", "ар",
            "ки", "сон", "мет", "ик", "са", "ба", "то", "дик", "жу", "хо", "од",
            "инт", "ко", "ласт", "ун", "кол", "ан", "но", "пит", "ру", "ит", "аст",
            "ант", "лос", "му", "тел", "ист", "ам", "род", "ин", "ник", "кин", "ра",
            "рог", "ла", "ва", "ск", "ти", "ход", "лов", "тик", "метр", "ас"
        };
    }
}