using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI[] texts = new TMPro.TextMeshProUGUI[3];
    [SerializeField] float delay;
    float counter = 0f;
    Dictionary<TMPro.TextMeshProUGUI, string> hoverText;
    Dictionary<TMPro.TextMeshProUGUI, string> normalText;
    string[] nextText = { "Start", "Guide", "Quit"};
    bool[] isHover = {false, false, false };
    int[] isTyping = {-1, -1, -1 };
    // Start is called before the first frame update
    void Start()
    {
        hoverText = new Dictionary<TMPro.TextMeshProUGUI, string>()
        {
            {texts[0] , "let action = StartGame();"},
            {texts[1], "let action = Learn();" },
            {texts[2], "let action = Exit();" }
        };

        normalText = new Dictionary<TMPro.TextMeshProUGUI, string>()
        {
            {texts[0] , "Start"},
            {texts[1], "Guide" },
            {texts[2], "Quit" }
        };
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i<3; i++)
        {
            if (isTyping[i] == 0)
            {
                texts[i].text = "" + nextText[i].ToCharArray()[0];
                isTyping[i]++;
            }
            if (isTyping[i] >= nextText[i].Length)
            {
                continue;
            }
            if (isTyping[i]>0 && counter>=delay)
            {
                texts[i].text = texts[i].text + nextText[i].ToCharArray()[isTyping[i]];
                isTyping[i]++;
                counter = 0f;
            }
        }
        counter += Time.deltaTime;
    }

    public void TypingEffect(int textIndex)
    {
        isTyping[textIndex] = 0;
        isHover[textIndex] = !isHover[textIndex];
        texts[textIndex].gameObject.transform.parent.gameObject.GetComponent<Image>().color = new Color(0.7f,0.7f,0.7f,isHover[textIndex]?1:0);
        nextText[textIndex] = isHover[textIndex] ? hoverText[texts[textIndex]] : normalText[texts[textIndex]];
    }

    public void onClick(int index)
    {
        if (index == 0)
        {
            SceneManager.LoadScene("Level1-Introducing Variables");
        }
        if (index == 1)
        {
            return;
        }
        if (index == 2)
        {
            Application.Quit();
        }
    }
}
