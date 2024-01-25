using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GuideData", menuName = "ScriptableObjects/Tutorial")]
public class GuideData : ScriptableObject
{
    [TextArea(3, 15)] public string codeText;
    [TextArea(3, 15)] public string descriptionText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
