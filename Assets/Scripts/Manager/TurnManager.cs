using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private TileBoard boardPlayer;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private Button battleButton;
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
        Events.Instance.OnLittleBattleStart += LittleBattleStart;

        battleButton.interactable = false;
        battleButton.onClick.AddListener(() =>
        {
            // ����
            battleButton.interactable = false;
            Events.Instance.LittleBattleStart();
        });
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
        // ���ӻغ���
        round++;
        roundText.text = round.ToString();

        // ÿ10�غϽ���һ��
        if (round % 10 ==0)
        {
            // �򿪿�ս��ť
            battleButton.interactable = true;
            Events.Instance.BattleStart();
        }
    }

    private void LittleBattleStart()
    {
        StartCoroutine(WaitLittleBattleChanges());
    }

    IEnumerator WaitLittleBattleChanges()
    {
        yield return new WaitForSeconds(0.5f);
        Events.Instance.LittleBattleEnd();
    }

    public void NewGame()
    {
        round = 0;
        roundText.text = round.ToString();
    }
}
