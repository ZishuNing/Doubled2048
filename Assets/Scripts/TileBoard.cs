using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Button PlayerSkillButton;

    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting = false;
    private bool IsCanUseSkill
    {
        get => isCanUseSkill;
        set {
            isCanUseSkill = value;
            PlayerSkillButton.interactable = value;
        }
    }
    private bool isCanUseSkill = true;
    private int SkillCount
    {
        get => skillCount;
        set
        {
            skillCount = value;
            IsCanUseSkill = skillCount > 0 ? false : true;
        }
    }
    private int skillCount = 0;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
        Events.Instance.OnGameStart += NewGame;
        Events.Instance.OnBattleStart += StartBattle;
        Events.Instance.OnLittleBattleStart += StartLittleBattle;
        Events.Instance.OnBattleEnd += EndBattle;
        Events.Instance.OnTileDead += OnTileDead;
        Events.Instance.OnGenerateExtraUnit += CreateTile;
        Events.Instance.OnEnemyDeadChooseEnd += EnemyDeadChooseEnd;
    }

    private void Start()
    {
        PlayerSkillButton.onClick.AddListener(() =>
        {
            PlayerSkill();
        });
    }

    private void OnDestroy()
    {
        Events.Instance.OnGameStart -= NewGame;
        Events.Instance.OnBattleStart -= StartBattle;
        Events.Instance.OnLittleBattleStart -= StartLittleBattle;
        Events.Instance.OnBattleEnd -= EndBattle;
        Events.Instance.OnTileDead -= OnTileDead;
        Events.Instance.OnGenerateExtraUnit -= CreateTile;
        Events.Instance.OnEnemyDeadChooseEnd -= EnemyDeadChooseEnd;
    }

    private void OnTileDead(TileModel model)
    {
        tiles.RemoveAll(tile => tile.model == model);
    }

    private void NewGame()
    {
        // update board state
        this.ClearBoard();
        this.CreateTile();
        this.CreateTile();
    }

    private void StartBattle()
    {
        waiting = true;
        IsCanUseSkill = false;
    }

    private void StartLittleBattle()
    {
        // 开始战斗，先把所有块向右移动
        MoveWithoutMerge(Vector2Int.right, grid.Width - 2, -1, 0, 1);
        StartCoroutine(WaitMoveAndRegisterDamage());
    }

    private void EndBattle()
    {
        waiting = false;
        IsCanUseSkill = true;
    }

    private void Update()
    {
        if (waiting) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Vector2Int.up, 0, 1, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left, 1, 1, 0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
        }
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells) {
            cell.tile = null;
        }

        foreach (var tile in tiles) {
            if (tile != null) Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        GameObject go = Instantiate(tilePrefab, grid.transform);
        Tile tile = go.GetComponent<Tile>();
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    public void CreateTile(Vector2Int coordinates,int tileLevel = 1)
    {
        TileCell cell = grid.GetCell(coordinates);
        if (cell != null && !cell.Occupied)
        {
            GameObject go = Instantiate(tilePrefab, grid.transform);
            Tile tile = go.GetComponent<Tile>();
            tile.Spawn(cell, tileLevel);
            tiles.Add(tile);
        }
    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;
        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied) {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed)
            StartCoroutine(WaitForMove());
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

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
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

    private bool CanMerge(Tile a, Tile b)
    {
        return a.model.CurLevel == b.model.CurLevel && !b.locked && a.state.unitType == b.state.unitType && a.model.CurLevel<a.state.maxLevel;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);
        b.Upgrade();
        Events.Instance.Merge();
    }

    private IEnumerator WaitForMove()
    {
        waiting = true;

        yield return new WaitForSeconds(0.15f);

        waiting = false;

        foreach (var tile in tiles) {
            tile.locked = false;
        }

        if (tiles.Count != grid.Size)
        {
            CreateTile();
        }

        //等
        yield return new WaitForSeconds(0.05f);

        if (tiles.Count != grid.Size)
        {
            // 移动完成后触发事件
            Events.Instance.TurnEnd();
        }
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.Size) {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }

        return true;
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
        //            score += TilesManager.Instance.GetPowerOfTwo(cell.tile.state.number); ;
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
                if (cell.Occupied && cell.tile.model.CurAttack>0)
                {
                    //Debug.Log("Register Damage: " + cell.coordinates + ", Damage: " + cell.tile.state.attack);
                    Tile targetTile = BattleManager.Instance.FindNearestTargetTile(cell.coordinates, PlayerType.Player);
                    if (targetTile == null) continue;
                    int distance = BattleManager.Instance.GetDistanceX(cell.coordinates, targetTile.cell.coordinates, PlayerType.Player);
                    if (distance > cell.tile.model.CurAttackRange) continue;
                    BattleManager.Instance.RegisterDamagePlayer(cell.tile, targetTile, cell.tile.model.CurAttack, cell.tile.state.unitType);
                }
            }
        }
    }

    // 玩家技能，给最后列添加一级单位
    private void PlayerSkill()
    {
        if (!IsCanUseSkill || SkillCount>0) return;
        int summonLevel = BuffManager.Instance.buffSkillsummonedUnitLevel + BattleManager.Instance.playerConfig.baseSkillsummonedUnitLevel;
        for (int y = 0; y < grid.Height; y++)
        {
            CreateTile(new Vector2Int(grid.Width - 1, y), summonLevel);
        }
        SkillCount++;
    }

    private void EnemyDeadChooseEnd()
    {
        SkillCount = 0;
        IsCanUseSkill = true;
    }
}
