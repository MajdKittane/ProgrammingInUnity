using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_InputField input;
    Environment env;
    [SerializeField]
    GameObject platform;
    [SerializeField]
    float speed;
    bool run = false;
    bool dir = false;
    // Start is called before the first frame update
    void Start()
    {
        env = new Environment();
        env.Set("run", new Boolean { value = false });
    }

    // Update is called once per frame
    void Update()
    {
        if (platform.transform.position == new Vector3(8f, -0.4f, 3f))
        {
            dir = true;
        }
        if (platform.transform.position == new Vector3(-2f, -0.4f, 3f))
        {
            dir = false;
        }
        if (run)
        {
            if (dir)
            {
                platform.transform.position = Vector3.MoveTowards(platform.transform.position, new Vector3(-2f, -0.4f, 3f), speed * Time.deltaTime);
            }
            else
            {
                platform.transform.position = Vector3.MoveTowards(platform.transform.position, new Vector3(8f, -0.4f, 3f), speed * Time.deltaTime);
            }
        }
    }
    private void FixedUpdate()
    {
        
    }

    public void Run()
    {
        Lexer lexer = new Lexer(input.text);
        Parser parser = new Parser(lexer);
        Program p = parser.ParseProgram();
        Evaluator.Eval(p, env);
        if (((Boolean)env.store["run"]).value == true)
        {
            run = true;
        }
        else
        {
            run = false;
        }    
    }
}
