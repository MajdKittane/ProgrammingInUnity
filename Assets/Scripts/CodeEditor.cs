using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;

public class CodeEditor : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField shown;
    [SerializeField] TMPro.TMP_InputField hidden;
    List<Token> tokens = new();
    List<int> tokensLocation = new();
    string previousFrameText = "";
    Dictionary<TokenType, string> colorPerTokenType = new Dictionary<TokenType, string>
    {
        { TokenType.Let,"<color=#E7755B>"},
        { TokenType.Identifier,"<color=#5bcde7>"},
        { TokenType.If,"<color=#E7755B>"},
        { TokenType.Else,"<color=#E7755B>"},
        { TokenType.Loop,"<color=#E7755B>"}
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (hidden.text.Replace("\n","").Replace("\t","").Replace("\r","").Replace(" ","") != previousFrameText.Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""))
        {
            CodeRefactoring();
        }
        ColorHighlighting();
        previousFrameText = hidden.text;
    }

    void CodeRefactoring()
    {
        List<string> lines = new();
        string line = "";
        for (int i = 0; i < hidden.text.Length; i++)
        {
            line += hidden.text[i];
            if (hidden.text[i] == '\n')
            {
                lines.Add(line);
                line = "";
                continue;
            }

        }
        if (line != "")
        {
            lines.Add(line);
        }

        int l = 0;
        string tempText = "";
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains("}"))
            {
                if (l != 0) l--;
            }

            lines[i] = HandleSpacing(lines[i], l);

            if (lines[i].Contains("{"))
            {
                l++;
            }

            tempText += lines[i];
        }
        if (hidden.text != tempText)
        {
            hidden.text = tempText;
            hidden.caretPosition = hidden.text.Length;
        }


    }

    void ColorHighlighting()
    {
        tokens = new();
        hidden.text = hidden.text.ToLower();
        string editedText = hidden.text;
        Lexer lexer = new Lexer(hidden.text);
        Token token = lexer.NextToken();

        while (true)
        {
            if (token.tokenType == TokenType.EOF)
            {
                break;
            }
            tokens.Add(token);
            tokensLocation.Add(lexer.position - 1);
            token = lexer.NextToken();
        }

        editedText = "";
        string buildingToken = "";
        int nextToken = 0;

        for (int i = 0; i < hidden.text.Length; i++)
        {
            if (hidden.text[i] == ' ' || hidden.text[i] == '\n' || hidden.text[i] == '\t' || hidden.text[i] == '\r')
            {
                editedText += hidden.text[i];
                buildingToken = "";
                continue;
            }
            buildingToken += hidden.text[i];
            if (buildingToken == tokens[nextToken].literal)
            {
                if (colorPerTokenType.ContainsKey(tokens[nextToken].tokenType))
                {
                    buildingToken = buildingToken.Insert(0, colorPerTokenType[tokens[nextToken].tokenType]);
                    buildingToken += "</color>";
                }


                editedText += buildingToken;
                buildingToken = "";
                nextToken++;
                if (nextToken >= tokens.Count) break;
            }
        }
        shown.text = editedText;
    }


    string HandleSpacing(string str, int level)
    {
        int numOfSpaces = level * 4;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == ' ')
            {
                numOfSpaces--;
            }
        }
        for (int i = 0; i < numOfSpaces; i++)
        {
            str = str.Insert(0, " ");
        }
        return str;
    }    
}
