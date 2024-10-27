using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;

    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting = false;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
        Events.Instance.OnGameStart += NewGame;
        Events.Instance.OnBattleStart += StartBattle;
        Events.Instance.OnLittleBattleStart += StartLittleBattle;
        Events.Instance.OnBattleEnd += EndBattle;
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
    }

    private void OnDestroy()
    {
        Events.Instance.OnGameStart -= NewGame;
        Events.Instance.OnBattleStart -= StartBattle;
        Events.Instance.OnLittleBattleStart -= StartLittleBattle;
        Events.Instance.OnBattleEnd -= EndBattle;
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
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(TilesManager.Instance.GetTileState(0));
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied) {
                    MoveTile(cell.tile, direction);
                }
            }
        }
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
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(TilesManager.Instance.IndexOfTileStates(b.state) + 1, 0, TilesManager.Instance.GetTileStatesLength() - 1);
        TileState newState = TilesManager.Instance.GetTileState(index);

        b.SetState(newState);
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
        List<int> rowScore = new List<int>();

        for (int y = 0; y < grid.Height; y++)
        {
            int score = 0;

            for (int x = 0; x < grid.Width; x++)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied)
                {
                    score += TilesManager.Instance.GetPowerOfTwo(cell.tile.state.number); ;
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
                    //Debug.Log("Register Damage: " + cell.coordinates + ", Damage: " + cell.tile.state.attack);
                    Tile targetTile = BattleManager.Instance.FindNearestTargetTile(cell.coordinates, PlayerType.Player);
                    if (targetTile == null) continue;
                    int distance = BattleManager.Instance.GetDistanceX(cell.coordinates, targetTile.cell.coordinates, PlayerType.Player);
                    if (distance > cell.tile.model.attackRange) continue;
                    BattleManager.Instance.RegisterDamage(targetTile, cell.tile.state.attack, PlayerType.Player);
                }
            }
        }
    }
}
