using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] private TileGrid gridPlayer;
    [SerializeField] private TileGrid gridEnemy;

    // 测试用生命值
    [SerializeField] private TextMeshProUGUI PlayerHP;
    [SerializeField] private TextMeshProUGUI EnemyHP;
    private int playerHP = 10;
    private int enemyHP = 10;


    // Document the damage dealt to each tile
    private Dictionary<Tile,int> damageDocument = new Dictionary<Tile, int>();

    private void Start()
    {
        Events.Instance.OnLittleBattleEnd += EndLittleBattle;
        Events.Instance.OnBattleEnd += EndBattle;
        Events.Instance.OnGameStart += NewGame;

        // 测试用生命值
        UpdateHPUI();
    }

    private void NewGame()
    {
        playerHP = 10;
        enemyHP = 10;
        UpdateHPUI();
    }

    private void EndLittleBattle()
    {
        if (CheckIsBattleDone())
        {
            Events.Instance.BattleEnd();
        }
        else
        {
            DealDamageToTile();
            Events.Instance.LittleBattleStart();
        }
    }

    private void EndBattle()
    {
        DealDamageToHero();
        // 测试用生命值
        UpdateHPUI();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Events.Instance.OnBattleEnd -= EndLittleBattle;
        Events.Instance.OnBattleEnd -= EndBattle;
    }

    public Tile FindNearestTargetTile(Vector2Int myCoordinate, PlayerType playerType)
    {
        TileGrid tileGrid = playerType == PlayerType.Player ? gridEnemy : gridPlayer;

        Tile nearestEnemy = null;
        int myY = myCoordinate.y;
        int startX = playerType == PlayerType.Player ? 0 : tileGrid.Width - 1;
        for (int i = 0; i < tileGrid.Width; i++)
        {
            TileCell cell = tileGrid.GetCell(startX, myY);
            startX += playerType == PlayerType.Player ? 1 : -1;
            if (cell.Occupied && cell.tile.state.number > 1)
            {
                nearestEnemy = cell.tile;
                break;
            }
        }
        return nearestEnemy;
    }

    public int GetDistanceX(Vector2Int myCoordinate, Vector2Int targetCoordinate, PlayerType playerType)
    {
        if (playerType == PlayerType.Player)
        {
            return gridPlayer.Width - myCoordinate.x + targetCoordinate.x;
        }
        else
        {
            return gridPlayer.Width - targetCoordinate.x + myCoordinate.x;
        }
    }

    public void RegisterDamage(Tile tile, int damage, PlayerType playerType)
    {
        if (damageDocument.ContainsKey(tile))
        {
            damageDocument[tile] += damage;
        }
        else
        {
            damageDocument.Add(tile, damage);
        }
    }

    private void DealDamageToTile()
    {
        foreach (var tileDamage in damageDocument)
        {
            int index = TilesManager.Instance.IndexOfTileStates(tileDamage.Key.state);
            index -= tileDamage.Value;
            if (index < 0)
            {
                tileDamage.Key.DestroyTile();
            }
            else
            {
                tileDamage.Key.SetState(TilesManager.Instance.GetTileState(index));
            }
        }
        damageDocument.Clear();
    }

    private void DealDamageToHero()
    {
        int playerDamage = GetAllDamage(gridPlayer);
        int enemyDamage = GetAllDamage(gridEnemy);
        enemyHP -= playerDamage;
        playerHP -= enemyDamage;
    }

    private int GetAllDamage(TileGrid tileGrid)
    {
        int height = tileGrid.Height;
        int width = tileGrid.Width;
        int totalDamage = 0;
        for (int y = 0; y < height; y++)
        {
            int damage = 0;
            for (int x = 0; x < width; x++)
            {
                TileCell cell = tileGrid.GetCell(x, y);
                if (cell.Occupied)
                {
                    damage += TilesManager.Instance.GetPowerOfTwo(cell.tile.state.number);
                }
            }
            totalDamage += damage;
        }
        return totalDamage;
    }

    //如果没有任何注册的伤害，则战斗结束
    private bool CheckIsBattleDone()
    {
       return damageDocument.Count == 0;
    }

    private void UpdateHPUI()
    {
        PlayerHP.text = playerHP.ToString();
        EnemyHP.text = enemyHP.ToString();
    }
}
