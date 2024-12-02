using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Events : MonoBehaviour
{
    public static Events Instance { get; private set; }

    //��Ϸ��ʼ�¼�
    public event Action OnGameStart;
    //�غϽ����¼�
    public event Action OnTurnEnd;
    //ÿ10�غϴ���һ�ν����¼�
    public event Action OnBattleStart;
    public event Action OnLittleBattleStart;
    public event Action OnLittleBattleEnd;
    public event Action OnBattleEnd;

    //Tile�����¼�
    public event Action<TileModel> OnTileDead;
    //Tile����ֵ�仯�¼�
    public event Action<TileModel> OnTileHPChange;
    //Tile�����¼�
    public event Action<TileModel> OnTileLevelChange;
    //Tile�����¼�
    public event Action<TileDamage> OnTileAttack;
    //�ϲ��¼�
    public event Action OnMerge;

    //��������¼�
    public event Action OnPlayerDead;
    //���������¼�
    public event Action OnEnemyDead;
    //����������������ѡһ�¼�
    public event Action OnEnemyDeadChooseEnd;
    //�����¸���������¼�
    public event Action OnLoadNextEnemyEnd;

    //��Ϸ�����¼�
    public event Action OnGameEnd;

    //BUFF�¼�
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
