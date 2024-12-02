using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageToFrontRow : BuffBase
{
    public override void OnMerge()
    {
        // 对前排造成2点伤害
        Events.Instance.DealDamageToFrontRow(2);
    }
}
