using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileState state { get; private set; }
    public TileCell cell { get; private set; }
    public bool locked { get; set; }
    public TileModel model { get; private set; }

    private Image background;
    private TextMeshProUGUI text;

    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        model = GetComponent<TileModel>();
    }

    public void SetState(TileState state)
    {
        this.state = state;
        SetRandomAttackRange(this);

        if (model.attackRange > 1)
            background.color = Color.yellow;
        else
            background.color = Color.gray;
        //background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = state.number.ToString();
    }

    public void SetRandomAttackRange(Tile tile)
    {
        // 有0.7的概率攻击范围为1，0.3的概率攻击范围为3
        tile.model.attackRange = UnityEngine.Random.Range(0, 10) < 7 ? 1 : 3;
    }

    public void Spawn(TileCell cell)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        StartCoroutine(Animate(cell.transform.position, false));
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = null;
        cell.tile.locked = true;

        StartCoroutine(Animate(cell.transform.position, true));
    }

    public void DestroyTile()
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = null;
        Destroy(gameObject);
    }

    private IEnumerator Animate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (merging) {
            Destroy(gameObject);
        }
    }

}
