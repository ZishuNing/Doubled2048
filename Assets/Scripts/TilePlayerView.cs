using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TilePlayerView : TileView
{
    public override void RefreshUI()
    {
        base.RefreshUI();
    }

    protected override void OnTileAttack(TileDamage damage)
    {
        if (damage.Attacker == tile)
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
    }
}
