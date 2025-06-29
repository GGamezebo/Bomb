using System.Collections.Generic;
using UnityEngine;

namespace Archive
{
    public class UserPreferenceData
    {
        public List<string> players = new List<string>();
    }

    public static class UserPreferenceKeys
    {
        public const string PLAYERS_COUNT = "PLAYERS_COUNT";
        public const string PLAYER = "PLAYER_{0:0}";
    }

    public static class UserPreference
    {
        public static UserPreferenceData CreateDefaultData()
        {
            UserPreferenceData data = new UserPreferenceData();
            if (data.players.Count == 0)
            {
                data.players.Add("Player1");
                data.players.Add("Player2");
                data.players.Add("Player3");
            }

            return data;
        }

        public static void Save(UserPreferenceData data)
        {
            PlayerPrefs.DeleteAll();
        
            var playerCount = data.players.Count;
            if (playerCount > 0)
            {
                PlayerPrefs.SetInt(UserPreferenceKeys.PLAYERS_COUNT, playerCount);
                for (int i = 0; i < playerCount; ++i)
                {
                    PlayerPrefs.SetString(string.Format(UserPreferenceKeys.PLAYER, i), data.players[i]);
                }
            }

            PlayerPrefs.Save();
        }
        public static UserPreferenceData Load()
        {
            var data = CreateDefaultData();
            if (PlayerPrefs.HasKey(UserPreferenceKeys.PLAYERS_COUNT))
            {
                List<string> players = new List<string>();
                var playersCount = PlayerPrefs.GetInt(UserPreferenceKeys.PLAYERS_COUNT);
                for (int i = 0; i < playersCount; ++i)
                {
                    string player = PlayerPrefs.GetString(string.Format(UserPreferenceKeys.PLAYER, i), "");
                    if (player.Length > 0)
                    {
                        players.Add(player);
                    }
                }
                data.players = players;
            }
            return data;
        }
    }
}