using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using UnityEngine.UI;
using System.Threading;

public class ASCIIConversionPuzzle : AbstractPuzzle, Observer
{
    MazeLogic mazeLogic;
    [SerializeField] TMPro.TextMeshProUGUI onDoorText;
    [SerializeField] Door door;
    

    //Transition between threads.
    bool isSplittingText, splittedText = false;
    bool isUpdatingText, updatedText = false;
    bool getNextPuzzle = false;

    //Data transfered between threads.
    string[] nums;

    //Managing Description.
    int nextDescription = 1;
    int descriptionDetails = -1;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        mazeLogic = gameObject.GetComponent<MazeLogic>();
        mazeLogic.triggerManager.inputButton.onClick.AddListener(() =>
        {
            SavePlayerName();
            levelManager.Resume();
            mazeLogic.triggerManager.inputField.transform.root.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (isSplittingText && !splittedText)
        {
            nums = onDoorText.text.Split("\t");
            splittedText = true;
        }


        if (isUpdatingText && !updatedText)
        {
            string newText = "";
            foreach (string str in nums)
            {
                newText += str + "\t";
            }
            onDoorText.text = newText;
            updatedText = true;
        }

        if (getNextPuzzle)
        {
            mazeLogic.NextPuzzle();
        }
    }

    public override void Action()
    {
        if (!mazeLogic.triggerManager.GetActiveTrigger())
        {
            return;
        }

        if (mazeLogic.triggerManager.GetActiveTrigger().interactObject.GetComponent<Door>() is Door door)
        {
            if (mazeLogic.playerName == "")
            {
                mazeLogic.triggerManager.inputField.transform.root.gameObject.SetActive(true);
                levelManager.Pause();
            }

            if (levelManager.codeSaved)
            {
                RunCode();
                InteractTrigger active = mazeLogic.triggerManager.GetActiveTrigger();
                mazeLogic.triggerManager.OnInteractTrigger(active,false);
                active.gameObject.SetActive(false);
            }
        }

        if (mazeLogic.triggerManager.GetActiveTrigger().interactObject.GetComponentInChildren<WallText>() is WallText text)
        {

            mazeLogic.triggerManager.fullScreenText.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text.fullText;
            mazeLogic.triggerManager.fullScreenText.transform.root.gameObject.SetActive(true);
            levelManager.Pause();

            if (!mazeLogic.triggerManager.interactDone[mazeLogic.triggerManager.GetActiveTrigger()])
            {
                descriptionDetails++;
                UpdateDescription(nextDescription);
                nextDescription++;
            }
        }
    }

    public void RunCode()
    {
        List<MyInterpreter.Object> chars = new List<MyInterpreter.Object>();
        for (int i =0; i<mazeLogic.playerName.Length; i++)
        {
            chars.Add(new Integer { value= ((int)mazeLogic.playerName.ToCharArray()[i]) >= 97 ? ((int)mazeLogic.playerName.ToCharArray()[i]) - 97 : ((int)mazeLogic.playerName.ToCharArray()[i]) - 39 });
        }
        env.Set("n",new Integer { value = mazeLogic.playerName.Length });
        env.Set("a",new Array { elements = chars });
        thread.Start();
    }

    public override void CheckResult()
    {
        isSplittingText = true;
        splittedText = false;
        Thread.Sleep(100);

        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[0][0] != mazeLogic.playerName[0])
            {
                levelManager.Lose();
            }
        }
        door.OpenDoor();

        getNextPuzzle = true;
    }

    public void SavePlayerName()
    {
        string text = "";
        foreach (char ch in mazeLogic.triggerManager.inputField.GetComponent<TMPro.TMP_InputField>().text)
        {
            if (!IsASCIILetter(ch))
            {
                return;
            }
            text += ((int)ch) >= 97 ? ((int)ch) - 97 : (((int)ch) - 39).ToString();
            text += "\t";
        }

        if (text == "") return;

        mazeLogic.playerName = mazeLogic.triggerManager.inputField.GetComponent<TMPro.TMP_InputField>().text;
        onDoorText.text = text;
        descriptionDetails++;
        UpdateDescription(0);

    }

    void UpdateDescription(int details)
    {

        switch(details)
        {
            case 0:
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "Instead of your name, some numbers are printed on the door.";
                break;

            case 1:
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "Messages printed on walls around show numbers in them.";
                break;

            case 2:
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "Compare messages with numbers on the door.";
                break;

            default:
                break;
        }

        if (descriptionDetails >=2 )
        {
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "Input:";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "n = number of characters in your name";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "a[ ] = array of integer values of your name's characters.";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "0 <= a[ ] <= 51";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "Output:";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "a[ ] = array of right integer values of characters.";
            mazeLogic.saveCodeButton.interactable = true;
            mazeLogic.saveCodeButton.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Save Code";
        }
    }

    public void OnLoopIterationEnd()
    {
        return;
    }

    public void OnLoopEnd()
    {
        return;
    }

    public void OnLetStatement(string name, MyInterpreter.Object value, List<Integer> indexes)
    {
        if (name == "a" && indexes != null)
        {
            isSplittingText = true;
            splittedText = false;
            Thread.Sleep(300);

            int index = (int)indexes[0].value;
            nums[index] = IsASCIILetter((int)((Integer)value).value) ? ((char)((Integer)value).value).ToString() : Mathf.Abs(((Integer)value).value).ToString();

            isUpdatingText = true;
            updatedText = false;

            Thread.Sleep(500);
        }
    }

    public void OnBlockEnd()
    {
        return;
    }

    public void OnProgramEnd()
    {
        CheckResult();
        Thread.Sleep(500);
    }

    public bool IsASCIILetter(int num)
    {
        if (num >= 97 && num <= 122)
        {
            return true;
        }

        if (num >= 65 && num <= 90)
        {
            return true;
        }

        return false;
    }
}
