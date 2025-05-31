using System.Collections.Generic;
using Archive;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button startBattleButtonComponent;

        [SerializeField]
        private TMPro.TMP_InputField palyer1Component;
   
        [SerializeField]
        private TMPro.TMP_InputField palyer2Component;
   
        [SerializeField]
        private TMPro.TMP_InputField palyer3Component;
   
        [SerializeField]
        private TMPro.TMP_InputField palyer4Component;
   
        [SerializeField]
        private TMPro.TMP_InputField palyer5Component;
   
        [SerializeField]
        private TMPro.TMP_InputField palyer6Component;
   
        [SerializeField]
        private TMPro.TMP_InputField palyer7Component;
   
        [SerializeField]
        private TMPro.TMP_InputField palyer8Component;

        List<TMPro.TMP_InputField> players;

        // Start is called before the first frame update
        void Start()
        {
            players = new List<TMPro.TMP_InputField>();

            players.Add(palyer1Component);
            players.Add(palyer2Component);
            players.Add(palyer3Component);
            players.Add(palyer4Component);
            players.Add(palyer5Component);
            players.Add(palyer6Component);
            players.Add(palyer7Component);
            players.Add(palyer8Component);

            var data = UserPreference.Load();
            for (int i = 0; i < data.players.Count; ++i)
            {
                players[i].text = data.players[i];
            }

            startBattleButtonComponent.onClick.AddListener(this.OnStart);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnStart()
        {
            UserPreferenceData playersList = new UserPreferenceData();
            foreach(var player in players)
            {
                if (player.text.Length > 0)
                {
                    playersList.players.Add(player.text);
                }
            }
            UserPreference.Save(playersList);
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }




    }
}
