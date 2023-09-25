using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;

public class PotionMixingGame : Interpreter
{
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    public TMPro.TMP_InputField input;
    Vector3 nextPlace;
    Environment env;
    Environment nextEnv;
    Dictionary<string,GameObject> spawnedObjects;
    // Start is called before the first frame update
    void Start()
    {
        spawnedObjects = new Dictionary<string, GameObject>();
        nextPlace = new Vector3(-8.7f,-1.7f,-1f);

        env = new Environment();
        env.Set("x", new Integer() { value = 3 });
        env.Set("y", new Integer() { value = 5 });
        env.Set("z", new Integer() { value = 7 });
        foreach(string id in env.store.Keys)
        {
            spawnedObjects[id]=(Instantiate(prefab,nextPlace,Quaternion.identity));
            //spawnedObjects[id].transform.localScale *= ((Integer)(env.store[id])).value;
            spawnedObjects[id].GetComponentInChildren<TMPro.TextMeshPro>().text = id + " = " + ((Integer)(env.store[id])).value;
            nextPlace += new Vector3(3f,0f,0f);
        }
        nextEnv = env;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CompileCode()
    {
        Lexer lexer = new Lexer(input.text);
        Parser parser = new Parser(lexer);
        Program p = parser.ParseProgram();


        Debug.Log(CheckLetStatements(p.statements));


        Evaluator.Eval(p, nextEnv);

        foreach (string id in nextEnv.store.Keys)
        {
            if (!spawnedObjects.ContainsKey(id))
            {
                spawnedObjects[id] = (Instantiate(prefab, nextPlace, Quaternion.identity));
                //spawnedObjects[id].transform.localScale *= ((Integer)(env.store[id])).value;
                spawnedObjects[id].GetComponentInChildren<TMPro.TextMeshPro>().text = id + " = " + ((Integer)(nextEnv.store[id])).value;
                nextPlace += new Vector3(3f, 0f, 0f);
            }
        }
    }
    bool CheckLetStatements(List<Statement> statements)
    {
        foreach (Statement statement in statements)
        {
            if (statement is LetStatement lt)
            {
                if (lt.value is InfixExpression exp)
                {
                    if (exp.op == "+" && exp.left.ToString() == "r" && (exp.right.ToString() == "x" || exp.right.ToString() == "y" || exp.right.ToString() == "z"))
                    {
                        continue;
                    }
                    return false;
                }
                return false;
            }
            else
            {
                if (statement is ExpressionStatement exp)
                {
                    if (exp.expression is LoopExpression loop)
                    {
                        if(!CheckLetStatements(loop.body.statements))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
}
