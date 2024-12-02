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
                    StartCoroutine(DelayedBowLandingEffect());
                    break;
            }
        }
    }

    private IEnumerator DelayedBowLandingEffect()
    {
        // 延迟0.5秒
        yield return new WaitForSeconds(1f);

        // 实例化BowLandingEffect
        GameObject instantiatedBowLanding = Instantiate(TilesManager.Instance.BowLandingEffect);
        instantiatedBowLanding.transform.SetParent(transform);
        instantiatedBowLanding.transform.localPosition = new Vector3(0, 278, 0);
        instantiatedBowLanding.transform.localScale = new Vector3(0.75f, 0.75f, 0);

        // 销毁效果对象
        Destroy(instantiatedBowLanding, 1f);
        // 或者使用对象池释放效果对象
        // StartCoroutine(ReleaseEffectAfterTime(instantiatedBowLanding, TilesManager.Instance.BowLandingEffectPool, 1f));
    }
}