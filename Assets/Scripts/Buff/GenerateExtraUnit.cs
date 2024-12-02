using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateExtraUnit : BuffBase
{
    private int turnCount = 0;
    public override void OnTurnEnd()
    {
        turnCount++;
        if (turnCount % 3 == 0)
        {
            // ����һ������ĵ�λ
            Events.Instance.GenerateExtraUnit();
        }
    }
}
