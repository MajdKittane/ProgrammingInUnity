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
    bool done = false;
    int scale = 1;
    Vector3 end;
    float elapsedTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if (done)
        {
            elapsedTime += Time.deltaTime;
            float p = elapsedTime / 5;
            outputTransform.transform.localScale = Vector3.Lerp(new Vector3(1,1,1),end,p);
            if (outputTransform.transform.localScale == end) done = false;
        }
    }

    public void RunCode()
    {
        done = false;
        elapsedTime = 0f;
        scale = 1;
        var lexer = new Lexer(code.text);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        Environment env = new Environment();
        Evaluator.Eval(program, env);

        scale = int.Parse(env.store["out"].Inspect());
        done = true;
        end = new Vector3(scale,scale,scale);
        
    }
}
