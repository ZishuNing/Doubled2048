using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    }

    private void Start()
    {
        Events.Instance.OnTurnEnd += EndTurn;
        Events.Instance.OnGameStart += NewGame;
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        Events.Instance.OnTurnEnd -= EndTurn;
        Events.Instance.OnGameStart -= NewGame;
    }

    public void EndTurn()
    {
        // 增加回合数
        round++;
        roundText.text = round.ToString();

        // 每10回合结算一次
        if (round % 10 ==0)
        {
            // 结算
            Events.Instance.Settlement();
        }
    }

    public void NewGame()
    {
        round = 0;
        roundText.text = round.ToString();
    }
}
