using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyInterpreter;
using System.Threading;

public class CAESARPuzzle : AbstractPuzzle, Observer
{
    [SerializeField] TMPro.TextMeshProUGUI hiddenWallText;
    [SerializeField] GameObject[] wallTexts = new GameObject[3];
    MazeLogic mazeLogic;
    int encryptKey;
    int[] solution = new int[3];
    string encryptedPlayerName = "";
    List<string> allWords = new List<string>(new string[8] { "IntEntIOn", "InfOrmAtIOn", "tEmpErAtUrE", "prOtEctIOn", "cOntrIbUtIOn", "LItErAtUrE", "cOnfUsIOn", "UndErstAndIng" });
    List<string> selectedWords = new List<string>();
    int textWorkingOn = 0;


    //Transition between threads.
    bool isSplittingText, splittedText = false;
    bool isUpdatingText, updatedText = false;

    //Data transfered between threads.
    string[] textCharArray;
    int currentIndex;

    //Managing Description.
    int nextDescription = 1;
    int descriptionDetails = -1;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        mazeLogic = gameObject.GetComponent<MazeLogic>();
        encryptKey = GenerateKey();
        solution = GenerateTexts();
        encryptedPlayerName = Encrypt(mazeLogic.playerName, encryptKey);
        hiddenWallText.GetComponent<TMPro.TextMeshProUGUI>().text = "Your Name is :\n" + encryptedPlayerName;
        UpdateDescription(0);
        mazeLogic.triggerManager.inputButton.onClick.RemoveAllListeners();
        mazeLogic.triggerManager.inputButton.onClick.AddListener(() =>
        {
            CheckResult();
        });
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (isSplittingText && !splittedText)
        {
            textCharArray = new string[wallTexts[textWorkingOn].GetComponent<TMPro.TextMeshProUGUI>().text.Length];
            for (int i=0; i< wallTexts[textWorkingOn].GetComponent<TMPro.TextMeshProUGUI>().text.Length; i++)
            {
                textCharArray[i] = wallTexts[textWorkingOn].GetComponent<TMPro.TextMeshProUGUI>().text[i].ToString();
            }
            splittedText = true;
        }


        if (isUpdatingText && !updatedText)
        {
            wallTexts[textWorkingOn].GetComponent<TMPro.TextMeshProUGUI>().text = "";
            for (int i = 0; i < textCharArray.Length; i++)
            {
                wallTexts[textWorkingOn].GetComponent<TMPro.TextMeshProUGUI>().text += textCharArray[i];
            }
            updatedText = true;
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

            mazeLogic.triggerManager.inputField.transform.root.gameObject.SetActive(true);
            levelManager.Pause();
        }

        if (mazeLogic.triggerManager.GetActiveTrigger().interactObject.GetComponentInChildren<WallText>() is WallText text)
        {
            if (!levelManager.codeSaved)
            {
                mazeLogic.triggerManager.fullScreenText.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text.fullText;
                mazeLogic.triggerManager.fullScreenText.transform.root.gameObject.SetActive(true);
                levelManager.Pause();
            }

            if (!mazeLogic.triggerManager.interactDone[mazeLogic.triggerManager.GetActiveTrigger()])
            {
                descriptionDetails++;
                UpdateDescription(nextDescription);
                nextDescription++;
            }

            if (levelManager.codeSaved)
            {
                for (int i=0; i<3; i++)
                {
                    if (text == wallTexts[i].GetComponent<WallText>())
                    {
                        textWorkingOn = i;
                        RunCode(textWorkingOn);
                        return;
                    }
                }
            }


        }
    }

    public void RunCode(int textIndex)
    {
        List<MyInterpreter.Object> chars = new List<MyInterpreter.Object>();
        for (int i = 0; i < wallTexts[textIndex].GetComponent<WallText>().fullText.Length; i++)
        {
            if (!IsASCIILetter(wallTexts[textIndex].GetComponent<WallText>().fullText[i]))
            {
                chars.Add(new Integer { value = ((int)wallTexts[textIndex].GetComponent<WallText>().fullText[i]) + 51 });
                continue;
            }
            chars.Add(new Integer { value = ((int)wallTexts[textIndex].GetComponent<WallText>().fullText[i]) >= 97 ? ((int)wallTexts[textIndex].GetComponent<WallText>().fullText[i]) - 97 : ((int)wallTexts[textIndex].GetComponent<WallText>().fullText[i]) - 39 });
        }
        Debug.Log(((Integer)chars[0]).value);
        env.Set("n", new Integer { value = wallTexts[textIndex].GetComponent<WallText>().fullText.Length });
        env.Set("a", new Array { elements = chars });
        wallTexts[textIndex].GetComponent<WallText>().enabled = false;
        wallTexts[textIndex].gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = wallTexts[textIndex].GetComponent<WallText>().fullText;
        thread.Start();
    }

    public override void CheckResult()
    {
        for (int i =0; i< mazeLogic.triggerManager.inputField.GetComponent<TMPro.TMP_InputField>().text.Length; i++)
        {
            if (selectedWords[i][solution[i]] != mazeLogic.triggerManager.inputField.GetComponent<TMPro.TMP_InputField>().text[i])
            {
                levelManager.Lose();
            }
        }
        levelManager.Win();
    }

    void UpdateDescription(int details)
    {

        switch (details)
        {
            case 0:
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "Your name ? is show on a wall.";
                break;

            case 1:
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "Messages on walls look like they are encrypted.";
                break;

            case 2:
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "\n";
                mazeLogic.levelDescription.text += "Try to decrypt messages to take instructions from them.";
                break;

            default:
                break;
        }

        if (descriptionDetails >= 2)
        {
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "Input:";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "n = number of characters in the message";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "a[ ] = array of integer values of message's characters.";
            mazeLogic.levelDescription.text += "0 <= a[ ] <= 51 for letters";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "Others values (numbers/spaces) must not be edited, ignore them.";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "Output:";
            mazeLogic.levelDescription.text += "\n";
            mazeLogic.levelDescription.text += "a[ ] = array of right integer values of characters after decryption.";
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
            if (IsASCIILetter((int)((Integer)value).value))
            {
                textCharArray[index] = ((char)((Integer)value).value).ToString();
            }
            else
            {
                int notLetter = (int)((Integer)value).value - 51;
                textCharArray[index] = IsNumberOrSpace(notLetter) ? ((char)notLetter).ToString() : Mathf.Abs(notLetter).ToString();
            }
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
        return;
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

    public bool IsNumberOrSpace(int num)
    {
        if (num >= 48 && num <= 57)
        {
            return true;
        }

        if (num == 32 || num == 10)
        {
            return true;
        }

        return false;
    }

    int GenerateKey()
    {
        int key = Random.Range(20,77);
        if (key % 52 == 0) return GenerateKey();
        return key;
    }

    string Encrypt(string plain, int key)
    {
        string encrypted = "";
        foreach (char plainChar in plain)
        {
            if (!char.IsLetter(plainChar))
            {
                encrypted += plainChar;
                continue;
            }
            int plainInt = ((int)plainChar) >= 97 ? ((int)plainChar) - 97 : ((int)plainChar) - 39;
            int cipherInt = (plainInt + key) % 52;
            char cipherChar = cipherInt >= 26 ? ((char)(cipherInt + 39)) : ((char)(cipherInt + 97));
            encrypted += cipherChar;
        }
        return encrypted;
    }

    int[] GenerateTexts()
    {
        int[] sol = new int[3];
        for (int i=0; i<3; i++)
        {
            int randomIndex = Random.Range(4,9);
            sol[i] = randomIndex - 1;
            wallTexts[i].GetComponent<WallText>().fullText = "The " + randomIndex + "th LEtter Of fOlLOwIng word Is the ";
            if (i == 0) wallTexts[i].GetComponent<WallText>().fullText += "1st"; if (i == 1) wallTexts[i].GetComponent<WallText>().fullText += "2nd"; if (i == 2) wallTexts[i].GetComponent<WallText>().fullText += "3rd";
            wallTexts[i].GetComponent<WallText>().fullText += " LEtTer Of thE pAsSwOrd.\n";
            randomIndex = Random.Range(0, allWords.Count);
            wallTexts[i].GetComponent<WallText>().fullText += allWords[randomIndex];
            selectedWords.Add(allWords[randomIndex]);
            allWords.RemoveAt(randomIndex);
            wallTexts[i].GetComponent<WallText>().fullText = Encrypt(wallTexts[i].GetComponent<WallText>().fullText,encryptKey);
        }
        return sol;
    }
}
