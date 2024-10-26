using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tile State")]
public class TileState : ScriptableObject
{
    public int number;
    public Color backgroundColor;
    public Color textColor;

    public float attack = 0.5f;
    public int attackRange = 1;
}
