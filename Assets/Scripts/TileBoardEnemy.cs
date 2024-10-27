using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoardEnemy : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private List<LevelConfig> levelConfigs;

    private LevelConfig levelConfig;
    private int CurrentLevel=0;
    private TileGrid grid;
    private List<Tile> tiles;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
        Events.Instance.OnGameStart += NewGame;
        Events.Instance.OnLittleBattleStart += StartLittleBattle;
        Events.Instance.OnBattleEnd += BattleEnd;
    }

    private void NewGame()
    {
        // update board state
        this.ClearBoard();
        CurrentLevel = 0;
        foreach (var tile in levelConfigs[CurrentLevel].tiles)
        {
            this.CreateSpecificTile(tile);
        }
    }

    private void StartLittleBattle()
    {
        // 开始战斗，先把所有块向左移动
        MoveWithoutMerge(Vector2Int.left, 1, 1, 0, 1);
        StartCoroutine(WaitMoveAndRegisterDamage());
    }

    private void BattleEnd()
    {
        CurrentLevel++;
        levelConfig = levelConfigs[CurrentLevel % levelConfigs.Count];

        foreach (var tile in levelConfig.tiles)
        {
            this.CreateSpecificTile(tile);
        }
    }

    private void OnDestroy()
    {
        Events.Instance.OnGameStart -= NewGame;
        Events.Instance.OnLittleBattleStart -= StartLittleBattle;
        Events.Instance.OnBattleEnd -= BattleEnd;
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            if (tile != null) Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateSpecificTile(TileState tileState)
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileState);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void MoveWithoutMerge(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied)
                {
                    MoveTileWithoutMerge(cell.tile, direction);
                }
            }
        }
    }

    private void MoveTileWithoutMerge(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
        }
    }

    // 获取每一行的分数
    public List<int> GetRowScore()
    {
        List<int> rowScore = new List<int>();

        for (int y = 0; y < grid.Height; y++)
        {
            int score = 0;

            for (int x = 0; x < grid.Width; x++)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied)
                {
                    score += TilesManager.Instance.GetPowerOfTwo(cell.tile.state.number);
                }
            }

            rowScore.Add(score);
        }

        return rowScore;
    }
    

    IEnumerator WaitMoveAndRegisterDamage()
    {
        yield return new WaitForSeconds(0.15f);
        RegisterDamage();
    }

    // 我方给敌人造成伤害
    public void RegisterDamage()
    {
        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.Occupied && cell.tile.state.number > 1)
                {
                    Tile targetTile = BattleManager.Instance.FindNearestTargetTile(cell.coordinates, PlayerType.Enemy);
                    if (targetTile == null) continue;
                    int distance = BattleManager.Instance.GetDistanceX(cell.coordinates, targetTile.cell.coordinates, PlayerType.Enemy);
                    if (distance > cell.tile.state.attackRange) continue;
                    BattleManager.Instance.RegisterDamage(targetTile, cell.tile.state.attack, PlayerType.Enemy);
                }
            }
        }
    }

    //设置敌人关卡
    public void SetLevel(int level)
    {
        levelConfig = levelConfigs[level];
    }
}
