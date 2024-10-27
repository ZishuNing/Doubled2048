using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tile State")]
public class TileState : ScriptableObject
{
    public int number;
    public Color backgroundColor;
    public Color textColor;

    public int attack = 1;
    //public int attackRange = 1;
}
