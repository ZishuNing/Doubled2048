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
        Events.Instance.OnGameStart += CleanScore;
    }

    private void Start()
    {
        Events.Instance.OnTurnEnd += CalScore;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        Events.Instance.OnTurnEnd -= CalScore;
        Events.Instance.OnGameStart -= CleanScore;
    }

    public void CalScore()
    {
        //List<int> scorePlayer = boardPlayer.GetRowScore();
        //List<int> scoreEnemy = boardEnemy.GetRowScore();
        //for (int i = 0; i < scorePlayer.Count; i++)
        //{
        //    if (scorePlayer[i] > scoreEnemy[i])
        //    {
        //        rowText[i].color = Color.green;
        //        rowText[i].text = (scorePlayer[i] - scoreEnemy[i]).ToString();
        //    }
        //    else if (scorePlayer[i] < scoreEnemy[i])
        //    {
        //        rowText[i].color = Color.red;
        //        rowText[i].text = (scoreEnemy[i] - scorePlayer[i]).ToString();
        //    }
        //    else
        //    {
        //        rowText[i].color = Color.blue;
        //        rowText[i].text = (scorePlayer[i] - scoreEnemy[i]).ToString();
        //    }
        //}
    }

    //public List<int> GetAllScore()
    //{
    //    //List<int> scorePlayer = boardPlayer.GetRowScore();
    //    //List<int> scoreEnemy = boardEnemy.GetRowScore();
    //    //List<int> score = new List<int>();
    //    //for (int i = 0; i < scorePlayer.Count; i++)
    //    //{
    //    //    score.Add(scorePlayer[i] - scoreEnemy[i]);
    //    //}
    //    return score;
    //}

    public void CleanScore()
    {
        //foreach (var text in rowText)
        //{
        //    text.text = "";
        //}
    }
}
