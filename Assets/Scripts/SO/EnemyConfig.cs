using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public Texture avatar;
    public int hp;
    public List<LevelConfig> Levels;
}

