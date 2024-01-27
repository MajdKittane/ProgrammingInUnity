using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using MyInterpreter;

public abstract class AbstractPuzzle : MonoBehaviour
{
    //In Editor Testing
    [SerializeField] bool useTestInput;
    [TextArea(3, 10)] [SerializeField] string testInput;


    protected Thread thread;
    protected Environment env;
    protected bool isProgramDone = false;
    [HideInInspector] public LevelLogic levelManager { get; protected set; }

    // Start is called before the first frame update
    public virtual void Start()
    {
        BaseStart();
    }

    protected void BaseStart()
    {
        thread = new Thread(Run);
        env = new Environment();
        levelManager = FindAnyObjectByType<LevelLogic>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        BaseUpdate();
    }

    protected void BaseUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.timeScale != 0f)
        {
            Action();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            levelManager.Retry();
        }

        if (isProgramDone)
        {
            CheckResult();
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
