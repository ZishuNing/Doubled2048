using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadPanel : MonoBehaviour 
{
    private Image panel;

    private void Awake()
    {
        panel = GetComponent<Image>();
        panel.color = new Color(0, 0, 0, 1);
        panel.DOColor(new Color(0, 0, 0, 0), 0.5f);
    }

    private void Start()
    {
        Events.Instance.OnPlayerDead += PlayerDead;
        Events.Instance.OnEnemyDeadChooseEnd += EnemyDeadChooseEnd;
        Events.Instance.OnGameEnd += GameEnd;
    }

    private void OnDestroy()
    {
        Events.Instance.OnPlayerDead -= PlayerDead;
        Events.Instance.OnEnemyDeadChooseEnd -= EnemyDeadChooseEnd;
        Events.Instance.OnGameEnd -= GameEnd;
    }

    private void PlayerDead()
    {
        SceneManager.LoadScene("YOUDEAD");
    }

    private void EnemyDeadChooseEnd()
    {
        panel.DOColor(new Color(1, 0, 0, 1f), 1f).OnComplete(
            () =>
            {
                panel.DOColor(new Color(1, 0, 0, 0), 1f);
            });
    }

    private void GameEnd()
    {
        SceneManager.LoadScene("YOUWIN");
    }
}

