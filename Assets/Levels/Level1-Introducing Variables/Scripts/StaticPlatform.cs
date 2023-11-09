using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;

public class StaticPlatform : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_InputField input;
    Environment env;
    [SerializeField]
    GameObject platform;
    [SerializeField]
    float speed;
    long size = 1;


    // Start is called before the first frame update
    void Start()
    {
        env = new Environment();
        env.Set("run", new Integer { value = 1 });
    }

    // Update is called once per frame
    void Update()
    {
        if (platform.transform.localScale.x != size)
        {
            platform.transform.localScale = Vector3.MoveTowards(platform.transform.localScale, new Vector3(platform.transform.localScale.x, size, platform.transform.localScale.z), speed * Time.deltaTime);
        }
        
    }

    public void Run()
    {
        Lexer lexer = new Lexer(input.text);
        Parser parser = new Parser(lexer);
        Program p = parser.ParseProgram();
        Evaluator.Eval(p, env);
        size = ((Integer)env.store["run"]).value;
    }
}
