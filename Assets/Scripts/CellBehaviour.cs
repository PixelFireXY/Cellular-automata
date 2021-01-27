using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{
    public Color CellColor
    {
        get => cellColor;
        set
        {
            cellColor = value;

            spriteRenderer.color = cellColor;
        }
    }
    private Color cellColor;

    public bool IsCellAlive
    {
        get => isCellAlive;
        set
        {
            isCellAlive = value;

            CellColor = isCellAlive ? Color.black : Color.white;
        }
    }
    private bool isCellAlive;

    public Vector2Int CellIndexPos { get; set; }


    [SerializeField]
    private SpriteRenderer spriteRenderer = null;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = gameObject.GetOrAddComponent<SpriteRenderer>();
    }
}
