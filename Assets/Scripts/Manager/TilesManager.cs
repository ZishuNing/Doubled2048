using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TilesManager : Singleton<TilesManager>
{
    [SerializeField] private Sprite[] tileMeleeSprites;
    [SerializeField] private Sprite[] tileRangedSprites;
    private TileState tileMeleeStates;
    private TileState tileRangedStates;

    protected override void Awake()
    {
        base.Awake();
        // lOAD TILE STATES FROM STEAMING ASSETS
        tileMeleeStates = Addressables.LoadAssetAsync<TileState>("tileMeleeStates").WaitForCompletion();
        tileRangedStates = Addressables.LoadAssetAsync<TileState>("tileRangedStates").WaitForCompletion();
    }

    public TileState GetRandomInitState()
    {
        // 80% chance to get a melee unit
        return Random.value < 0.8f ? tileMeleeStates : tileRangedStates;
    }

    public Sprite GetSprite(int unitType,int level)
    {
        switch ((UnitType)unitType)
        {
            case UnitType.Melee:
                return tileMeleeSprites[level-1];
            case UnitType.Ranged:
                return tileRangedSprites[level-1];
            default:
                return null;
        }
    }
}
