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
            "кар",
            "co",
            "метр",
            "инк",
            "ем",
            "ан",
            "бри",
            "бы",
            "вни",
            "гон",
            "до",
            "доз",
            "жи",
            "зик",
            "кас",
            "кос",
            "кра",
            "мер",
            "мол",
            // "мор",
            // "пра",
            // "пре",
            // "при",
            // "рен",
            // "ска",
            // "сли",
            // "соль",
            // "тел",
            // "фи",
            // "ча",
            // "чик",
            // "чу",
            // "ши",
            // "ща",
            // "щик",
            // "щу",
            // "чер",
            // "шаб",
            // "кон",
            // "ноп",
            // "нос",
            // "май",
            // "сонь",
            // "лом",
            // "пка",
            // "тер",
            // "ран",
            // "от",
            // "обо",
            // "синь",
            // "мой",
            // "чок",
            // "кет",
            // "доч",
            // "кет",
            // "рад",
            // "лон",
            // "кат",
            // "лец",
            // "лат",
            // "ник",
            // "бен",
            // "вец",
            // "вод",
            // "нал",
            // "ряд",
            // "кет",
            // "ряд",
            // "кет",
            // "рат",
            // "рог",
            // "пог",
            // "пок",
            // "мя",
            // "бан",
            // "узд",
            // "пах",
            // "зем",
            // "пле",
            // "бра",
            // "зна",
            // "корь",
            // "жар",
            // "нец",
            // "нок",
            // "кав",
            // "нок",
            // "чаг",
            // "рик",
            // "хар",
            // "сан",
            // "виш",
            // "сли",
            // "сед",
            // "гор",
            // "шко",
            // "рох",
            // "мен",
            // "сце",
            // "рок",
            // "чаш",
            // "кус",
            // "лин",
            // "лод",
            // "фра",
            // "што",
            // "сня",
            // "лик",
            // "нец"
        };
    }
}