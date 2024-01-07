using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;

public class LevelLogic : MonoBehaviour
{
    [SerializeField] GameObject uiObject;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!uiObject.activeSelf)
            {
                uiObject.SetActive(true);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void SaveCode()
    {
        uiObject.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
