using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageToFrontRow : BuffBase
{
    public override void OnMerge()
    {
        // ��ǰ�����2���˺�
        Events.Instance.DealDamageToFrontRow(2);
    }
}
