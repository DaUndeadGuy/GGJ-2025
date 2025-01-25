using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class draggable : MonoBehaviour
{
    public const string TANGENT_TAG = "TanDrag";
    public const string INCORRECT_TAG = "IncDrag";
    public const string CORRECT_TAG = "CorDrag";
    public Text text;

    private bool dragging = false;

    private Vector2 currentPosition;

    private Transform objectToDrag;
    private Image objectToDragImage;


    List<RaycastResult> hitObjects = new List<RaycastResult>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            objectToDrag = GetDraggableTransformUnderMouse();
            if (objectToDrag != null)
            {
                dragging = true;

                objectToDrag.SetAsLastSibling();

                currentPosition = objectToDrag.position;
                objectToDragImage = objectToDrag.GetComponent<Image>();
                objectToDragImage.raycastTarget = false;
            }
        }

        if (dragging)
        {
            objectToDrag.position = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (objectToDrag != null)
            {
                Transform objectToCheck = GetDraggableTransformUnderMouse();

                if (objectToCheck != null)
                {
                    //text.text = "connected";
                    if(objectToDragImage.gameObject.tag == CORRECT_TAG && objectToCheck.gameObject.tag == CORRECT_TAG)
                    {
                        Destroy(objectToDragImage.gameObject);
                        objectToDrag.position = Input.mousePosition;
                    } else if (objectToDragImage.gameObject.tag == TANGENT_TAG || objectToCheck.gameObject.tag == TANGENT_TAG)
                    {
                        if (objectToDragImage.gameObject.tag == TANGENT_TAG) {
                            Destroy(objectToDragImage.gameObject);
                        }
                        else
                        {
                            Destroy(objectToCheck.gameObject);
                        }
                        objectToDrag.position = Input.mousePosition;
                    }
                    else
                    {
                        objectToDrag.position = currentPosition;
                    }
                        
                }
                else
                {
                    objectToDrag.position = Input.mousePosition;
                }

                objectToDragImage.raycastTarget = true;
                objectToDrag = null;
            }
            
            dragging = false;
        }
    }

    private GameObject GetObjectUnderMouse()
    {
        var pointer = new PointerEventData(EventSystem.current);

        pointer.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointer, hitObjects);

        if (hitObjects.Count <= 0 ) return null;

        return hitObjects.First().gameObject;
    }

    private Transform GetDraggableTransformUnderMouse()
    {
        GameObject clickedObject = GetObjectUnderMouse();

        if (clickedObject != null && (clickedObject.tag == TANGENT_TAG || clickedObject.tag == INCORRECT_TAG || clickedObject.tag == CORRECT_TAG))
        {
            return clickedObject.transform;
        }

        return null;
    }
}
