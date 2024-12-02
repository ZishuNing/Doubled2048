using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedUnitLevel3 : BuffBase
{
    public override void OnAdd()
    {
        BuffManager.Instance.buffSkillsummonedUnitLevel = 2;
    }
}
