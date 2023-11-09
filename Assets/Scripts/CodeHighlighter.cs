using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeHighlighter : MonoBehaviour
{
    Dictionary<string, string> colorTagTable = new Dictionary<string, string>{
        {"if", "#D8A0DF"},
        {"else", "#D8A0DF"},
        {"for", "#D8A0DF"},
        {"let", "#D8A0DF"},
        {"fn", "#D8A0DF"},
        {"true", "#D8A0DF"},
        {"false", "#D8A0DF"},
        {"return", "#D8A0DF"}
    };
    [SerializeField]
    TMPro.TMP_InputField inputField;
    [SerializeField]
    GameObject textContainer;
    [SerializeField]
    string[] words;
    void Start()
    {
        //inputField = GameObject.FindObjectOfType<TMPro.TMP_InputField>();
        //textContainer = GameObject.Find("Styled Text");
    }


    void Update()
    {
        
    }

    public void UpdateStyledText()
    {
        textContainer.GetComponent<TMPro.TMP_Text>().text = "";

        string[] lines = inputField.text.Split("\n");
        words = inputField.text.Split(' ');

        for (int i=0; i<lines.Length; i++)
        {
            words = lines[i].Split(" ");
            for (int j = 0; j < words.Length; j++)
            {

                words[j] = SkipTags(words[j]);
                if (GetColorTag(words, j) != "")
                    textContainer.GetComponent<TMPro.TMP_Text>().text += "<color=" + GetColorTag(words, j) + ">" + words[j] + "</color>" + " ";
                else
                    textContainer.GetComponent<TMPro.TMP_Text>().text += words[j] + " ";
                if (j == words.Length - 1) textContainer.GetComponent<TMPro.TMP_Text>().text += "\n";
            }
        }
    }

    string GetColorTag(string[] words,int i)
    {
        if (colorTagTable.ContainsKey(words[i].Trim()))
        {
            return colorTagTable[words[i]];
        }
        if (i>0)
        {
            if (words[i - 1].Trim() == "let")  return "#86DCF9";
        }
        return "";
    }

    string SkipTags(string word)
    {
        if (word.Contains("</color>")) word = word.Substring(word.IndexOf(">")+1,word.LastIndexOf("<")- word.IndexOf(">") -1);
        return word;
    }
}
