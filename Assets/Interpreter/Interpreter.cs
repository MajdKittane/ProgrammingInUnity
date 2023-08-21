using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;

public class Interpreter : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_InputField code;
    [SerializeField]
    GameObject outputTransform;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunCode()
    {
        var lexer = new Lexer(code.text);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        Environment env = new Environment();
        Evaluator.Eval(program, env);

        float newScale = int.Parse(env.store["out"].Inspect());
        outputTransform.transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
