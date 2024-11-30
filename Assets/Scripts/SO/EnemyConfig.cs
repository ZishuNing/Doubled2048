using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public Texture avatar;
    public List<LevelConfig> Levels;
}

