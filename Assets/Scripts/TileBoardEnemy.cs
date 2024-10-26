using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoardEnemy : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileState[] tileStates;
    [SerializeField] private LevelConfig levelConfig;

    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
        Events.Instance.OnGameStart += NewGame;
        Events.Instance.OnSettlement += StartBattle;
    }

    private void NewGame()
    {
        // update board state
        this.ClearBoard();
        foreach (var tile in levelConfig.tiles)
        {
            this.CreateSpecificTile(tile);
        }
    }

    private void StartBattle()
    {
        // 开始战斗，先把所有块向左移动
        MoveWithoutMerge(Vector2Int.left, 1, 1, 0, 1);
    }

    private void OnDestroy()
    {
        Events.Instance.OnGameStart -= NewGame;
        Events.Instance.OnSettlement -= StartBattle;
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
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
                    score += cell.tile.state.number;
                }
            }

            rowScore.Add(score);
        }

        return rowScore;
    }
}
