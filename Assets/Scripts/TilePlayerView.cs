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
                    GameObject instantiatedAxe = Instantiate(TilesManager.Instance.AxeEffect);//Pool.Get();
                    instantiatedAxe.transform.SetParent(transform);
                    instantiatedAxe.transform.localPosition = new Vector3(0, -19, 0);
                    instantiatedAxe.transform.localScale = Vector3.one;
                    Destroy(instantiatedAxe, 1f);
                    //StartCoroutine(ReleaseEffectAfterTime(instantiatedAxe, TilesManager.Instance.AxeEffectPool, 1f));
                    break;
                case (int)UnitType.Ranged:
                    GameObject instantiatedBow = Instantiate(TilesManager.Instance.BowEffect);//Pool.Get();
                    instantiatedBow.transform.SetParent(transform);
                    instantiatedBow.transform.localPosition = new Vector3(26.5f, 45.4f, 0);
                    instantiatedBow.transform.localScale = new Vector3(0.75f, 0.75f, 0);
                    Destroy(instantiatedBow, 1f);
                    //StartCoroutine(ReleaseEffectAfterTime(instantiatedBow, TilesManager.Instance.BowEffectPool, 1f));
                    break;
            }
        }
    }
}
