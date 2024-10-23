using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TileBoard boardPlayer;
    [SerializeField] private TileBoardEnemy boardEnemy;
    [SerializeField] private List<TextMeshProUGUI> rowText = new List<TextMeshProUGUI>();

    private int frameCounter = 0;  // 用于计数帧数
    private int framesPerCalculation = 5;  // 每5帧执行一次计算

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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        // 增加帧计数器
        frameCounter++;

        // 当帧计数达到指定的帧数时，进行计算
        if (frameCounter >= framesPerCalculation)
        {
            CalScore();
            frameCounter = 0;  // 重置帧计数器
        }
    }

    public void CalScore()
    {
        List<int> scorePlayer = boardPlayer.GetRowScore();
        List<int> scoreEnemy = boardEnemy.GetRowScore();
        for (int i = 0; i < scorePlayer.Count; i++)
        {
            if (scorePlayer[i] > scoreEnemy[i])
            {
                rowText[i].text = scorePlayer[i]+":"+scoreEnemy[i]+ " Win";
            }
            else if (scorePlayer[i] < scoreEnemy[i])
            {
                rowText[i].text = scorePlayer[i] + ":" + scoreEnemy[i] + "Lose";
            }
            else
            {
                rowText[i].text = scorePlayer[i] + ":" + scoreEnemy[i] + "Draw";
            }
        }
    }
}
