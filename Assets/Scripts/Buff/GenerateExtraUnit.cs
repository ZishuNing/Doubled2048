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
            // 生成一个额外的单位
            Events.Instance.GenerateExtraUnit();
        }
    }
}
