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
    protected TileModel model;
    protected Tile tile; 

    private void Awake()
    {
        avatar = GetComponent<Image>();
        HPbar = transform.Find("HPbar").GetComponent<Image>();
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
        avatar.sprite = TilesManager.Instance.GetSprite(model.state.unitType, model.CurLevel);
    }

    // Coroutine to release effects back to the pool
    //protected IEnumerator ReleaseEffectAfterTime(GameObject effect, ObjectPool<GameObject> pool, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    if (effect != null) // Check if the effect is still valid
    //    {
    //        pool.Release(effect);
    //    }
    //}
}
