using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    DealDamageToFrontRow,
    GenerateExtraUnit,
    SummonUnitLevel3,
}

public class BuffManager : Singleton<BuffManager>
{
    private List<BuffBase> buffs = new List<BuffBase>();
    [HideInInspector] public int buffSkillsummonedUnitLevel = 0;

    public void AddBuff(BuffType buffType)
    {
        BuffBase buff = null;
        switch (buffType)
        {
            case BuffType.DealDamageToFrontRow:
                buff = new DealDamageToFrontRow();
                break;
            case BuffType.GenerateExtraUnit:
                buff = new GenerateExtraUnit();
                break;
            case BuffType.SummonUnitLevel3:
                buff = new SummonedUnitLevel3();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(buffType), buffType, null);
        }
        buffs.Add(buff);
        buff.OnAdd();
    }

    private void Start()
    {
        Events.Instance.OnTurnEnd += TurnEnd;
        Events.Instance.OnMerge += Merge;
    }

    private void Merge()
    {
        foreach (var buff in buffs)
        {
            buff.OnMerge();
        }
    }

    private void TurnEnd()
    {
        foreach (var buff in buffs)
        {
            buff.OnTurnEnd();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Events.Instance.OnTurnEnd -= TurnEnd;
    }
}
