using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : Singleton<TilesManager>
{
    [SerializeField] private TileState tileMeleeStates;
    [SerializeField] private TileState tileRangedStates;

    public TileState GetRandomInitState()
    {
        // 80% chance to get a melee unit
        return Random.value < 0.8f ? tileMeleeStates : tileRangedStates;
    }
}
