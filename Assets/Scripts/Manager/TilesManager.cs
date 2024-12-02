using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.UI;

public class TilesManager : Singleton<TilesManager>
{
    [SerializeField] private Sprite[] tileMeleeSprites;
    [SerializeField] private Sprite[] tileRangedSprites;
    [SerializeField] private Sprite[] diceSprites;
    private TileState tileMeleeStates;
    private TileState tileRangedStates;

    // 武器预制体特效
    public GameObject AxeEffect;
    public GameObject BowEffect;
    public GameObject BowLandingEffect;
    public GameObject BiteEffect;

    protected override void Awake()
    {
        base.Awake();
        // lOAD TILE STATES FROM STEAMING ASSETS
        tileMeleeStates = Addressables.LoadAssetAsync<TileState>("tileMeleeStates").WaitForCompletion();
        tileRangedStates = Addressables.LoadAssetAsync<TileState>("tileRangedStates").WaitForCompletion();
        // Load effects and initialize pools
        Addressables.LoadAssetAsync<GameObject>("AxeEffect").Completed += handle =>
        {
            AxeEffect = handle.Result;
        };

        Addressables.LoadAssetAsync<GameObject>("BowEffect").Completed += handle =>
        {
            BowEffect = handle.Result;
        };

        Addressables.LoadAssetAsync<GameObject>("BowLandingEffect").Completed += handle =>
        {
            BowLandingEffect = handle.Result;
        };
        Addressables.LoadAssetAsync<GameObject>("BiteEffect").Completed += handle =>
        {
            BiteEffect = handle.Result;
        };
    }

    public TileState GetRandomInitState()
    {
        // 80% chance to get a melee unit
        return Random.value < 0.8f ? tileMeleeStates : tileRangedStates;
    }

    public Sprite GetUnitSprite(int unitType,int level)
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

    public Sprite GetDiceSprite(int diceValue)
    {
        return diceSprites[diceValue - 1];
    }
}
