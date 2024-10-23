using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private TileBoard boardPlayer;
    [SerializeField] private int maxRounds = 10;
    [SerializeField] private TextMeshProUGUI roundText;
    public int round { get; private set; } = 0;


    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }

        boardPlayer.OnTurnEnd += EndTurn;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void EndTurn()
    {
        // 增加回合数
        round++;
        roundText.text = round.ToString();
    }
}
