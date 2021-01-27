using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public static Action<Vector2Int> onCellClicked;

    [SerializeField]
    private Camera cameraRaycast = null;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(cameraRaycast.ScreenPointToRay(Input.mousePosition));

            Transform cell = rayHit.transform;

            if (cell != null &&
                cell.CompareTag("Cell"))
            {
                // Hit a cell
                if (cell.TryGetComponent(out CellBehaviour cellIndex))
                {
                    onCellClicked?.Invoke(cellIndex.CellIndexPos);
                }
            }
        }
    }
}
