using UnityEngine;

public class BubbleTrigger : MonoBehaviour
{
    public GameObject bubbleUi;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bubbleUi.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            bubbleUi.SetActive(true);
        }
    }
}
