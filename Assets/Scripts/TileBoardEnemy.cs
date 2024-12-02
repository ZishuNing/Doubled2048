using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBoardEnemy : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private List<EnemyConfig> enemyConfigs;
    [SerializeField] private RawImage enemyAvatar;

    private LevelConfig levelConfig;
    private int CurrentRound=0;
    private int CurrentEnemy=0;
    private TileGrid grid;
    private List<Tile> tiles;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
        Events.Instance.OnGameStart += NewGame;
        Events.Instance.OnLittleBattleStart += StartLittleBattle;
        Events.Instance.OnBattleEnd += BattleEnd;
        Events.Instance.OnTileDead += OnTileDead;
        Events.Instance.OnEnemyDeadChooseEnd += EnemyDeadChooseEnd;
    }

    private void OnDestroy()
    {
        Events.Instance.OnGameStart -= NewGame;
        Events.Instance.OnLittleBattleStart -= StartLittleBattle;
        Events.Instance.OnBattleEnd -= BattleEnd;
        Events.Instance.OnTileDead -= OnTileDead;
        Events.Instance.OnEnemyDeadChooseEnd -= EnemyDeadChooseEnd;
    }

    private void OnTileDead(TileModel model)
    {
        tiles.RemoveAll(tile => tile.model == model);
    }

    private void EnemyDeadChooseEnd()
    {
        this.ClearBoard();
        StartCoroutine(LoadNextEnemy());
    }

    private IEnumerator LoadNextEnemy()
    {
        yield return new WaitForSeconds(0.97f);
        CurrentRound = 0;
        CurrentEnemy++;
        if (CurrentEnemy >= enemyConfigs.Count)
        {
            // 敌人全部死亡
            CurrentEnemy = 0;
            Events.Instance.GameEnd();
        }

        // 更新敌人头像
        enemyAvatar.texture = enemyConfigs[CurrentEnemy].avatar;

        foreach (int tileLevel in enemyConfigs[CurrentEnemy].Levels[CurrentRound].tilesLevel)
        {
            this.CreateSpecificTile(tileLevel, enemyConfigs[CurrentEnemy].Levels[CurrentRound].onlyTwoGenerateRow);
        }

        Events.Instance.LoadNextEnemyEnd();
    }

    private void NewGame()
    {
        // update board state
        this.ClearBoard();
        CurrentRound = 0;
        CurrentEnemy = 0;
        enemyAvatar.texture = enemyConfigs[CurrentEnemy].avatar;
        foreach (int tileLevel in enemyConfigs[CurrentEnemy].Levels[CurrentRound].tilesLevel)
        {
            this.CreateSpecificTile(tileLevel, enemyConfigs[CurrentEnemy].Levels[CurrentRound].onlyTwoGenerateRow);
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
        // 要等是因为BattleManager在计算伤害
        StartCoroutine(GenerateNewLevel());
    }

    IEnumerator GenerateNewLevel()
    {
        yield return new WaitForSeconds(0.05f);
        CurrentRound++;
        if (CurrentRound >= enemyConfigs[CurrentEnemy].Levels.Count)
        {
            yield break;
        }

        levelConfig = enemyConfigs[CurrentEnemy].Levels[CurrentRound % enemyConfigs[CurrentEnemy].Levels.Count];

        foreach (int tileLevel in levelConfig.tilesLevel)
        {
            this.CreateSpecificTile(tileLevel, levelConfig.onlyTwoGenerateRow);
        }
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

    public void CreateSpecificTile(int tileLevel, bool onlyTwoGenerateRow = false)
    {
        GameObject go = Instantiate(tilePrefab, grid.transform);
        Tile tile = go.GetComponent<Tile>();
        TileCell cell = onlyTwoGenerateRow ? grid.GetRandomEmptyCellInTwoRow() : grid.GetRandomEmptyCell();
        tile.Spawn(cell, tileLevel);
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
        List<int> rowScore = new List<int>(4);

        //for (int y = 0; y < grid.Height; y++)
        //{
        //    int score = 0;

        //    for (int x = 0; x < grid.Width; x++)
        //    {
        //        TileCell cell = grid.GetCell(x, y);

        //        if (cell.Occupied)
        //        {
        //            score += TilesManager.Instance.GetPowerOfTwo(cell.tile.state.number);
        //        }
        //    }

        //    rowScore.Add(score);
        //}

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
                if (cell.Occupied && cell.tile.model.CurAttack > 1)
                {
                    Tile targetTile = BattleManager.Instance.FindNearestTargetTile(cell.coordinates, PlayerType.Enemy);
                    if (targetTile == null) continue;
                    int distance = BattleManager.Instance.GetDistanceX(cell.coordinates, targetTile.cell.coordinates, PlayerType.Enemy);
                    if (distance > cell.tile.model.CurAttackRange) continue;
                    BattleManager.Instance.RegisterDamageEnemy(cell.tile, targetTile, cell.tile.model.CurAttack, cell.tile.state.unitType);
                }
            }
        }
    }

    public EnemyConfig GetCurrentEnemy()
    {
        return enemyConfigs[CurrentEnemy];
    }

    //设置敌人关卡
    public void SetLevel(int level)
    {
        levelConfig = enemyConfigs[CurrentEnemy].Levels[level];
    }
}
