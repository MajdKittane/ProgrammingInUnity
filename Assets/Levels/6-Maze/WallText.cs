using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallText : MonoBehaviour
{
    [TextArea(3,10)]public string fullText;
    string shownText = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shownText = "";
        string[] lines = fullText.Split("\n");
        foreach (string line in lines)
        {
            if (line.Trim() != "") shownText += line.Substring(0, line.Length/2) + "...\n";
        }
        GetComponent<TMPro.TextMeshProUGUI>().text = shownText;
    }
}
