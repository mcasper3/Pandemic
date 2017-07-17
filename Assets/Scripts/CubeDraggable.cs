using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum CubeColor { BLUE, BLACK, YELLOW, RED, RESEARCH };

    public Transform parentToReturnTo;
    public Vector3 newPosition;
    public bool isStockCube;

    public GameObject prefab;
    public CubeColor cubeColor;

    private GameObject replacementCube;
    private Transform originalParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = this.transform.parent;
        originalParent = parentToReturnTo;

        this.newPosition = this.transform.position;

        if (isStockCube)
        {
            replacementCube = Instantiate<GameObject>(prefab);
            replacementCube.transform.SetParent(parentToReturnTo);
            replacementCube.transform.position = this.transform.position;

            replacementCube.GetComponent<CubeDraggable>().isStockCube = true;
            
            var image = replacementCube.GetComponent<Image>();

            switch (cubeColor)
            {
                case CubeColor.BLACK:
                    image.color = new Color(0, 0, 0);
                    break;
                case CubeColor.BLUE:
                    image.color = new Color(0, 0, 255);
                    break;
                case CubeColor.RED:
                    image.color = new Color(255, 0, 0);
                    break;
                case CubeColor.YELLOW:
                    image.color = new Color(255, 237, 0);
                    break;
            }
        }

        this.transform.SetParent(parentToReturnTo.parent);

        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        this.transform.position = newPosition;

        if (isStockCube && originalParent == parentToReturnTo)
        {
            Destroy(replacementCube);
        }
        else if (isStockCube)
        {
            this.isStockCube = false;
            this.originalParent.GetComponent<GameInfo>().UpdateCubeCounter(cubeColor, true);
        }
    }
}
