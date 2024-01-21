using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeLogic : MonoBehaviour
{
    [HideInInspector] public TriggerManager triggerManager;
    [HideInInspector] public string playerName = "";
    [HideInInspector] public TMPro.TextMeshProUGUI levelDescription;
    [HideInInspector] public Button saveCodeButton;
    ASCIIConversionPuzzle asciiPuzzle;
    CAESARPuzzle caesarPuzzle;
    // Start is called before the first frame update
    void Start()
    {

        asciiPuzzle = GetComponent<ASCIIConversionPuzzle>();
        caesarPuzzle = GetComponent<CAESARPuzzle>();
        triggerManager = gameObject.GetComponent<TriggerManager>();
        asciiPuzzle.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextPuzzle()
    {
        asciiPuzzle.enabled = false;
        caesarPuzzle.enabled = true;
    }
}
