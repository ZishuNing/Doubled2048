using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Events : MonoBehaviour
{
    public static Events Instance { get; private set; }

    //游戏开始事件
    public event Action OnGameStart;
    //回合结束事件
    public event Action OnTurnEnd;
    //每10回合触发一次结算事件
    public event Action OnBattleStart;
    public event Action OnLittleBattleStart;
    public event Action OnLittleBattleEnd;
    public event Action OnBattleEnd;

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

    public void GameStart()
    {
        OnGameStart?.Invoke();
    }

    public void TurnEnd()
    {
        OnTurnEnd?.Invoke();
    }

    public void BattleStart()
    {
        OnBattleStart?.Invoke();
    }

    public void LittleBattleStart()
    {
        OnLittleBattleStart?.Invoke();
    }

    public void LittleBattleEnd()
    {
        OnLittleBattleEnd?.Invoke();
    }

    public void BattleEnd()
    {
        OnBattleEnd?.Invoke();
    }
}
