using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool isOpening = false;
    bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpening && !isOpen)
        {
            transform.position -= new Vector3(0,2 * Time.deltaTime,0);
            if (transform.localPosition.y <= -12.5f)
            {
                isOpen = true;
            }
        }

        if (isOpen)
        {
            Destroy(this.gameObject);
        }
    }

    public void OpenDoor()
    {
        isOpening = true;
    }
}
