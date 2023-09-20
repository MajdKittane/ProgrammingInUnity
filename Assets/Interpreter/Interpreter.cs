using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;

public class Interpreter : MonoBehaviour
{
    public GameObject prefab;
    [SerializeField]
    TMPro.TMP_InputField code;
    [SerializeField]
    GameObject outputTransform;
    bool done = false;
    int num = 0;
    Vector3 end;
    float elapsedTime = 0f;
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
        done = false;
        elapsedTime = 0f;
        var lexer = new Lexer(code.text);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        Environment env = new Environment();
        Evaluator.Eval(program, env);

        num = int.Parse(env.store["num"].Inspect());
        done = true;
        for (int i = 0; i<num; i++)
        {
            Instantiate(prefab, new Vector3(0f, 100f, 0f), Quaternion.identity);
        }
    }
}
