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
    public Vector3 originalPosition;
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
        this.originalPosition = this.newPosition;

        if (isStockCube)
        {
            replacementCube = Instantiate<GameObject>(prefab);

            replacementCube.transform.SetParent(parentToReturnTo);
            replacementCube.transform.position = this.transform.position;

            var cubeDraggable = replacementCube.GetComponent<CubeDraggable>();
            cubeDraggable.isStockCube = true;
            cubeDraggable.cubeColor = this.cubeColor;
            
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
        else
        {
            byte eventCode;

            switch (cubeColor)
            {
                case CubeColor.BLACK:
                    eventCode = GameManager.CREATE_BLACK;
                    break;
                case CubeColor.BLUE:
                    eventCode = GameManager.CREATE_BLUE;
                    break;
                case CubeColor.RED:
                    eventCode = GameManager.CREATE_RED;
                    break;
                case CubeColor.YELLOW:
                    eventCode = GameManager.CREATE_YELLOW;
                    break;
                case CubeColor.RESEARCH:
                    eventCode = GameManager.CREATE_RESEARCH;
                    break;
                default:
                    eventCode = GameManager.CREATE_BLACK;
                    break;
            }

            float[] createPosition = new float[] { newPosition.x, newPosition.y, newPosition.z };

            Debug.Log("Sent event");
            PhotonNetwork.RaiseEvent(eventCode, createPosition, true, null);

            if (isStockCube)
            {
                this.isStockCube = false;
                this.originalParent.GetComponent<GameInfo>().UpdateCubeCounter(cubeColor, true);
            }
            else
            {
                float[] destroyPosition = new float[] { originalPosition.x, originalPosition.y, originalPosition.z };

                PhotonNetwork.RaiseEvent(GameManager.DESTROY_CUBE, destroyPosition, true, null);
            }
        }
    }
}
