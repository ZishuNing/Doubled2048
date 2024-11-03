using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileState state{get { return model.state; }}
    public TileCell cell { get; private set; }
    public bool locked { get; set; }
    public TileModel model { get; private set; }

    private Image avatar;
    private Image HPbar;

    private void Awake()
    {
        avatar = GetComponent<Image>();
        HPbar = transform.Find("HPbar").GetComponent<Image>();
        model = GetComponent<TileModel>();
    }

    private void Start()
    {
        Events.Instance.OnTileDead += OnTileDead;
        Events.Instance.OnTileHPChange += OnTileHPChange;
        Events.Instance.OnTileLevelChange += OnTileLevelChange;
    }

    private void OnDestroy()
    {
        Events.Instance.OnTileDead -= OnTileDead;
        Events.Instance.OnTileHPChange -= OnTileHPChange;
        Events.Instance.OnTileLevelChange -= OnTileLevelChange;
    }

    private void OnTileHPChange(TileModel model)
    {
        if (model == this.model)
        {
            RefreshUI();
        }
    }

    private void OnTileLevelChange(TileModel model)
    {
        if (model == this.model)
        {
            RefreshUI();
        }
    }

    private void OnTileDead(TileModel model)
    {
        if (model == this.model)
        {
            DestroyTile();
        }
    }

    public void Spawn(TileCell cell, int tileLevel=1)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
        this.model.state = TilesManager.Instance.GetRandomInitState();
        this.model.CurLevel = tileLevel;
        RefreshUI();
    }

    public void MoveTo(TileCell cell)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        StartCoroutine(Animate(cell.transform.position, false));
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = null;
        cell.tile.locked = true;

        StartCoroutine(Animate(cell.transform.position, true));
    }

    public void Upgrade()
    {
        model.Upgrade();
        RefreshUI();
    }

    public void DestroyTile()
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = null;
        Destroy(gameObject);
    }

    private IEnumerator Animate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (merging) {
            Destroy(gameObject);
        }
    }

    private void RefreshUI()
    {
        HPbar.fillAmount = (float)model.CurHealth / model.GetMaxHealth();
        avatar.sprite = TilesManager.Instance.GetSprite(model.state.unitType, model.CurLevel);
    }

}
