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

    //Tile死亡事件
    public event Action<TileModel> OnTileDead;
    //Tile生命值变化事件
    public event Action<TileModel> OnTileHPChange;
    //Tile升级事件
    public event Action<TileModel> OnTileLevelChange;
    //Tile攻击事件
    public event Action<TileDamage> OnTileAttack;
    //合并事件
    public event Action OnMerge;

    //玩家死亡事件
    public event Action OnPlayerDead;
    //敌人死亡事件
    public event Action OnEnemyDead;
    //敌人死亡后跳出三选一事件
    public event Action OnEnemyDeadChooseEnd;
    //加载下个敌人完成事件
    public event Action OnLoadNextEnemyEnd;

    //游戏结束事件
    public event Action OnGameEnd;

    //BUFF事件
    public event Action<int> OnDealDamageToFrontRow;
    public event Action OnGenerateExtraUnit;

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

    public void TileDead(TileModel tile)
    {
        OnTileDead?.Invoke(tile);
    }

    public void TileHPChange(TileModel tile)
    {
        OnTileHPChange?.Invoke(tile);
    }

    public void TileLevelChange(TileModel tile)
    {
        OnTileLevelChange?.Invoke(tile);
    }

    public void TileAttack(TileDamage tileDamage)
    {
        OnTileAttack?.Invoke(tileDamage);
    }

    public void Merge()
    {
        OnMerge?.Invoke();
    }

    public void PlayerDead()
    {
        OnPlayerDead?.Invoke();
    }

    public void EnemyDead()
    {
        OnEnemyDead?.Invoke();
    }

    public void EnemyDeadChooseEnd()
    {
        OnEnemyDeadChooseEnd?.Invoke();
    }

    public void LoadNextEnemyEnd()
    {
        OnLoadNextEnemyEnd?.Invoke();
    }

    public void GameEnd()
    {
        OnGameEnd?.Invoke();
    }

    public void DealDamageToFrontRow(int damage)
    {
        OnDealDamageToFrontRow?.Invoke(damage);
    }

    public void GenerateExtraUnit()
    {
        OnGenerateExtraUnit?.Invoke();
    }
}
