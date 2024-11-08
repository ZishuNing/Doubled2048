using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct TileDamage
{
    public Tile Attacker;
    public Tile Target;
    public int AttackerUnitType;
    public int Damage;
}

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] private TileGrid gridPlayer;
    [SerializeField] private TileGrid gridEnemy;

    // 测试用生命值
    [SerializeField] private TextMeshProUGUI PlayerHP;
    [SerializeField] private TextMeshProUGUI EnemyHP;
    [SerializeField] private CharacterConfig playerConfig;
    [SerializeField] private CharacterConfig enemyConfig;
    private int playerHP;
    private int enemyHP;


    // Document the damage dealt to each tile
    private List<TileDamage> PlayerDamageDocument = new List<TileDamage>();
    private List<TileDamage> EnemyDamageDocument = new List<TileDamage>();

    private void Start()
    {
        Events.Instance.OnLittleBattleEnd += EndLittleBattle;
        Events.Instance.OnBattleEnd += EndBattle;
        Events.Instance.OnGameStart += NewGame;

        // 测试用生命值
        playerHP = playerConfig.baseHealth;
        enemyHP = enemyConfig.baseHealth;
        UpdateHPUI();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Events.Instance.OnBattleEnd -= EndLittleBattle;
        Events.Instance.OnBattleEnd -= EndBattle;
        Events.Instance.OnGameStart -= NewGame;
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
            StartCoroutine(DealDamageAndNextBattle());
        }
    }

    IEnumerator DealDamageAndNextBattle()
    {
        foreach (TileDamage tileDamage in PlayerDamageDocument)
        {
            Events.Instance.TileAttack(tileDamage);
        }
        PlayerDamageDocument.Clear();
        yield return new WaitForSeconds(1f);
        foreach (TileDamage tileDamage in EnemyDamageDocument)
        {
            Events.Instance.TileAttack(tileDamage);
        }
        EnemyDamageDocument.Clear();
        yield return new WaitForSeconds(1f);
        Events.Instance.LittleBattleStart();
    }

    private void EndBattle()
    {
        DealDamageToHero();
        // 测试用生命值
        UpdateHPUI();
        // 棋子在对战中未死亡，则恢复满状态
        RecoverAllTilesState();
    }

    private void RecoverAllTilesState()
    {
        gridEnemy.RecoverTilesState();
        gridPlayer.RecoverTilesState();
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
            if (cell.Occupied && cell.tile.model.CurHealth > 0)
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

    public void RegisterDamagePlayer(Tile attacker, Tile target, int damage, int attackerUnitType)
    {
        TileDamage tileDamage = new TileDamage
        {
            Attacker = attacker,
            Target = target,
            AttackerUnitType = attackerUnitType,
            Damage = damage
        };
        PlayerDamageDocument.Add(tileDamage);
    }

    public void RegisterDamageEnemy(Tile attacker, Tile target, int damage, int attackerUnitType)
    {
        TileDamage tileDamage = new TileDamage
        {
            Attacker = attacker,
            Target = target,
            AttackerUnitType = attackerUnitType,
            Damage = damage
        };
        EnemyDamageDocument.Add(tileDamage);
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
                    damage += cell.tile.model.CurAttackToPlayer;
                }
            }
            totalDamage += damage;
        }
        return totalDamage;
    }

    //如果没有任何注册的伤害，则战斗结束
    private bool CheckIsBattleDone()
    {
       return PlayerDamageDocument.Count == 0;
    }

    private void UpdateHPUI()
    {
        PlayerHP.text = playerHP.ToString();
        EnemyHP.text = enemyHP.ToString();
    }
}
