using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
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
        Events.Instance.OnTileAttack += OnTileAttack;
    }

    private void OnDestroy()
    {
        Events.Instance.OnTileDead -= OnTileDead;
        Events.Instance.OnTileHPChange -= OnTileHPChange;
        Events.Instance.OnTileLevelChange -= OnTileLevelChange;
        Events.Instance.OnTileAttack -= OnTileAttack;
    }

    private void OnTileAttack(TileDamage damage)
    {
        if(damage.Attacker == this)
        {
            switch (damage.AttackerUnitType)
            {
                case (int)UnitType.Melee:
                    GameObject instantiatedAxe = TilesManager.Instance.AxeEffectPool.Get();
                    instantiatedAxe.transform.SetParent(transform);
                    instantiatedAxe.transform.localPosition = new Vector3(0, -19, 0);
                    instantiatedAxe.transform.localScale = Vector3.one;
                    StartCoroutine(ReleaseEffectAfterTime(instantiatedAxe, TilesManager.Instance.AxeEffectPool, 1f));
                    break;
                case (int)UnitType.Ranged:
                    GameObject instantiatedBow = TilesManager.Instance.BowEffectPool.Get();
                    instantiatedBow.transform.SetParent(transform);
                    instantiatedBow.transform.localPosition = new Vector3(26.5f, 45.4f, 0);
                    instantiatedBow.transform.localScale = new Vector3(0.75f, 0.75f, 0);
                    StartCoroutine(ReleaseEffectAfterTime(instantiatedBow, TilesManager.Instance.BowEffectPool, 1f));
                    break;
            }
        }

        if (damage.Target == this)
        {
            switch (damage.AttackerUnitType)
            {
                case (int)UnitType.Melee:
                    break;
                case (int)UnitType.Ranged:
                    GameObject instantiatedBowLanding = TilesManager.Instance.BowLandingEffectPool.Get();
                    instantiatedBowLanding.transform.SetParent(transform);
                    instantiatedBowLanding.transform.localPosition = new Vector3(0, 278, 0);
                    instantiatedBowLanding.transform.localScale = new Vector3(0.75f, 0.75f, 0);
                    StartCoroutine(ReleaseEffectAfterTime(instantiatedBowLanding, TilesManager.Instance.BowLandingEffectPool, 1f));
                    break;
            }
            model.TakeDamage(damage.Damage);
        }
    }

    // Coroutine to release effects back to the pool
    private IEnumerator ReleaseEffectAfterTime(GameObject effect, ObjectPool<GameObject> pool, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Release(effect);
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
