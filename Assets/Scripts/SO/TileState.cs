using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tile State")]
public class TileState : ScriptableObject
{
    public int baseAttack;
    public int baseHealth;
    public int baseAttackRange;
    public int upgradeAttack;
    public int upgradeHealth;
    public int unitType;
    public int maxLevel=6;
}
