using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public abstract class TileView : MonoBehaviour
{
    protected Image avatar;
    protected Image HPbar;
    protected Image dice;
    protected TileModel model;
    protected Tile tile; 

    private void Awake()
    {
        avatar = GetComponent<Image>();
        HPbar = transform.Find("HPbar").GetComponent<Image>();
        dice = transform.Find("DICE").GetComponent<Image>();
        model = GetComponent<TileModel>();
        tile = GetComponent<Tile>();
    }

    private void Start()
    {
        Events.Instance.OnTileAttack += OnTileAttack;
    }

    private void OnDestroy()
    {
        Events.Instance.OnTileAttack -= OnTileAttack;
    }

    protected abstract void OnTileAttack(TileDamage damage);

    public virtual void RefreshUI()
    {
        HPbar.fillAmount = (float)model.CurHealth / model.GetMaxHealth();
        avatar.sprite = TilesManager.Instance.GetUnitSprite(model.state.unitType, model.CurLevel);
        dice.sprite = TilesManager.Instance.GetDiceSprite(model.CurLevel);
    }
}
