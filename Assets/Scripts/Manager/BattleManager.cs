using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] private TileGrid gridPlayer;
    [SerializeField] private TileGrid gridEnemy;

    // Document the damage dealt to each tile
    private Dictionary<Tile,int> damageDocument = new Dictionary<Tile, int>();

    private void Start()
    {
        Events.Instance.OnBattleEnd += EndBattle;
    }

    private void EndBattle()
    {
        DealDamage();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
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
        //Debug.Log("Register Damage: " + tile.cell.coordinates + ", Damage: " + damage+" Type: "+playerType);
        if (damageDocument.ContainsKey(tile))
        {
            damageDocument[tile] += damage;
        }
        else
        {
            damageDocument.Add(tile, damage);
        }
    }

    public void DealDamage()
    {
        foreach (var item in damageDocument)
        {
            //item.Key.state.number -= item.Value;
            Debug.Log("Num: "+item.Key.cell.coordinates + ", Damage: "+item.Value);
        }
        damageDocument.Clear();
    }
}
