using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEnemyView : TileView
{
    public override void RefreshUI()
    {
        base.RefreshUI();
    }

    protected override void OnTileAttack(TileDamage damage)
    {
        if (damage.Target == tile)
        {
            switch (damage.AttackerUnitType)
            {
                case (int)UnitType.Melee:
                    break;
                case (int)UnitType.Ranged:
                    GameObject instantiatedBowLanding = Instantiate(TilesManager.Instance.BowLandingEffect);//Pool.Get();
                    instantiatedBowLanding.transform.SetParent(transform);
                    instantiatedBowLanding.transform.localPosition = new Vector3(0, 278, 0);
                    instantiatedBowLanding.transform.localScale = new Vector3(0.75f, 0.75f, 0);
                    Destroy(instantiatedBowLanding, 1f);
                    //StartCoroutine(ReleaseEffectAfterTime(instantiatedBowLanding, TilesManager.Instance.BowLandingEffectPool, 1f));
                    break;
            }
        }
    }
}
