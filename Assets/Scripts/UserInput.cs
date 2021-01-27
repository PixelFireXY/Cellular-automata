using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInput : MonoBehaviour
{
    public static Action<Vector2Int> onCellClicked;
    public static Action onPauseButtonClicked;

    [SerializeField]
    private Camera cameraRaycast = null;

    [SerializeField]
    private Text pauseButtonText = null;

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

    public void PauseLifeCycle()
    {
        onPauseButtonClicked?.Invoke();

        if (pauseButtonText.text == ">")
            pauseButtonText.text = "||";
        else
            pauseButtonText.text = ">";
    }
}
