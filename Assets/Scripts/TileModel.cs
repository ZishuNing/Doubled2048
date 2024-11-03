using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileModel : MonoBehaviour
{
    public TileState state {
        get { 
            return _state;
        }
        set{
            _state = value;
            CurHealth = value.baseAttack;
            CurAttack = value.baseHealth;
            CurLevel = 1;
            CurAttackRange = value.baseAttackRange;
        }
    }
    private TileState _state;

    public int CurHealth {
        get { return _curHealth; }
        set
        {
            _curHealth = value;
            if(_curHealth <= 0)
            {
                Events.Instance.TileDead(this);
            }
            else
            {
                Events.Instance.TileHPChange(this);
            }
        }
    }
    private int _curHealth;

    public int CurAttack{ get; private set; }
    public int CurLevel { 
        get { return _curLevel; } 
        set {
            if (value > state.maxLevel)
            {
                throw new Exception("Tile level is too high");
            }
            _curLevel = value;
            CurHealth = state.baseHealth + (value - 1) * state.upgradeHealth;
            CurAttack = state.baseAttack + (value - 1) * state.upgradeAttack;
            CurAttackRange = state.baseAttackRange;
            Events.Instance.TileLevelChange(this);
        }
    }
    private int _curLevel = 1;
    public int CurAttackRange { get; private set; }
    public int CurAttackToPlayer { get { return CurLevel; } }


    public void TakeDamage(int value)
    {
        CurHealth -= value;
    }

    public void Upgrade()
    {
        if(CurLevel >= state.maxLevel)
        {
            return;
        }
        CurHealth += state.upgradeHealth;
        CurAttack += state.upgradeAttack;
        CurLevel++;
    }

    public void Recover()
    {
        CurHealth = state.baseHealth + (CurLevel - 1) * state.upgradeHealth;
    }
}
