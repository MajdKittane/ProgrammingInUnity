using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyInterpreter;

public class MazeLogic : MonoBehaviour
{
    
    [HideInInspector] public TriggerManager triggerManager;
    [HideInInspector] public LevelLogic levelManager;
    [HideInInspector] public string playerName = "";
    [HideInInspector] public TMPro.TextMeshProUGUI levelDescription;
    [HideInInspector] public Button saveCodeButton;
    AbstractPuzzle[] puzzles = new AbstractPuzzle[2];
    
    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindAnyObjectByType<LevelLogic>();
        triggerManager = gameObject.GetComponent<TriggerManager>();
        puzzles[0] = GetComponent<ASCIIConversionPuzzle>();
        puzzles[0].enabled = true;
        puzzles[1] = GetComponent<CAESARPuzzle>();

        for (int i = 0; i < levelManager.mainUI.transform.GetChild(0).childCount; i++)
        {
            if (levelManager.mainUI.transform.GetChild(0).GetChild(i).name == "Description")
            {
                levelDescription = levelManager.mainUI.transform.GetChild(0).GetChild(i).gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            }

            if (levelManager.mainUI.transform.GetChild(0).GetChild(i).name == "SaveCodeButton")
            {
                saveCodeButton = levelManager.mainUI.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Button>();
                saveCodeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Explore to get more details";
                saveCodeButton.interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerManager.GetActiveTrigger())
        {
            levelManager.interactText.SetActive(true);
        }
        else
        {
            levelManager.interactText.SetActive(false);
        }
    }

    public void NextPuzzle()
    {
        levelManager.codeSaved = false;
        levelManager.input.text = "";
        levelDescription.text = "";
        saveCodeButton.interactable = false;
        saveCodeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Explore to get more details";
        Destroy(puzzles[0]);
        puzzles[1].enabled = true;
    }
}
