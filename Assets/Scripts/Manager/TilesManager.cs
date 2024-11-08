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
    private TileState tileMeleeStates;
    private TileState tileRangedStates;

    // 武器预制体特效
    public GameObject AxeEffect;
    public GameObject BowEffect;
    public GameObject BowLandingEffect;
    // Object pools
    //public ObjectPool<GameObject> AxeEffectPool;
    //public ObjectPool<GameObject> BowEffectPool;
    //public ObjectPool<GameObject> BowLandingEffectPool;

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
            //AxeEffectPool = new ObjectPool<GameObject>(() => Instantiate(AxeEffect),
            //                                           go => go.SetActive(true),
            //                                           go => go.SetActive(false),
            //                                           Destroy, true, 10, 50);
        };

        Addressables.LoadAssetAsync<GameObject>("BowEffect").Completed += handle =>
        {
            BowEffect = handle.Result;
            //BowEffectPool = new ObjectPool<GameObject>(() => Instantiate(BowEffect),
            //                                           go => go.SetActive(true),
            //                                           go => go.SetActive(false),
            //                                           Destroy, true, 10, 50);
        };

        Addressables.LoadAssetAsync<GameObject>("BowLandingEffect").Completed += handle =>
        {
            BowLandingEffect = handle.Result;
            //BowLandingEffectPool = new ObjectPool<GameObject>(() => Instantiate(BowLandingEffect),
            //                                                 go => go.SetActive(true),
            //                                                 go => go.SetActive(false),
            //                                                 Destroy, true, 10, 50);
        };
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
