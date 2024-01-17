using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using MyInterpreter;

public abstract class AbstractPuzzle : MonoBehaviour
{
    [SerializeField] bool useTestInput;
    [TextArea(3, 10)] [SerializeField] string testInput;
    
    protected Thread thread;
    protected Environment env;
    protected LevelLogic levelManager;
    // Start is called before the first frame update
    public virtual void Start()
    {
        thread = new Thread(Run);
        env = new Environment();
        levelManager = FindAnyObjectByType<LevelLogic>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Action();
        }
    }


    public void Run()
    {
        Lexer lexer = new Lexer(useTestInput ? testInput : levelManager.input.text);
        Program prog = new Parser(lexer).ParseProgram();
        Evaluator.Eval(prog, env, this is Observer?(Observer)this:null);
    }
    public abstract void Action();
    public abstract void CheckResult();
}
