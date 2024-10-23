using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TileBoard boardPlayer;
    [SerializeField] private TileBoard boardEnemy;
    [SerializeField] private List<TextMeshProUGUI> rowText = new List<TextMeshProUGUI>();

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
        List<int> scorePlayer = boardPlayer.GetRowScore();
        List<int> scoreEnemy = boardEnemy.GetRowScore();
        for (int i = 0; i < scorePlayer.Count; i++)
        {
            if (scorePlayer[i] > scoreEnemy[i])
            {
                rowText[i].text = "Win";
            }
            else if (scorePlayer[i] < scoreEnemy[i])
            {
                rowText[i].text = "Lose";
            }
            else
            {
                rowText[i].text = "Draw";
            }
        }
    }
}
