using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Serializable]
    public class PlayerBars
    {
        public PlayerId playerId;
        public Slider healthBar;
        public Slider waterBar;
    }

    [SerializeField] private PlayerBars[] bars;

    private void Start()
    {
        var players = FindObjectsByType<PlayerResourceUI>(
            FindObjectsSortMode.None
        );

        foreach (var player in players)
        {
            foreach (var entry in bars)
            {
                if (entry.playerId != player.Id) continue;

                player.BindBars(entry.healthBar, entry.waterBar);
                break;
            }
        }
    }
}