using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;

public class Freeplay : MonoBehaviour
{
    [SerializeField]
    float TIME_THRESHOLD = 0f;
    [SerializeField]
    List<GameObject> bindableObjects;
    [SerializeField]
    TMPro.TMP_Dropdown idDropDown;
    [SerializeField]
    TMPro.TMP_Dropdown objectDropDown;
    [SerializeField]
    TMPro.TMP_Dropdown relationDropDown;
    [SerializeField]
    TMPro.TMP_Dropdown boundDropDown;
    [SerializeField]
    TMPro.TMP_InputField input;

    Environment env;
    Dictionary<string,KeyValuePair<GameObject,Bound>> bounds;
    Vector3 nextPlace;
    int[] objectCount;
    bool runEveryFrame = false;
    float timeSinceLastRun = 0f;
    public enum Relation
    {
        Scale,
        ScaleX,
        ScaleY,
        ScaleZ,
        Rot,
        RotX,
        RotY,
        RotZ
    }
    // Start is called before the first frame update
    void Start()
    {
        nextPlace = GameObject.Find("World").transform.position + new Vector3(-3f,0f,0f);
        bounds = new Dictionary<string, KeyValuePair<GameObject, Bound>>();
        objectCount = new int[bindableObjects.Count];
        env = new Environment();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeSinceLastRun > TIME_THRESHOLD) timeSinceLastRun = 0;
        if (runEveryFrame && timeSinceLastRun == 0)
        {
            Run();
        }
        timeSinceLastRun += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        
    }

    public void Stop()
    {
        runEveryFrame = false;
    }

    public void RunEveryFrame()
    {
        runEveryFrame = true;
    }

    public void CheckForVariables()
    {
        Environment tempEnv = new Environment();
        Lexer lexer = new Lexer(input.text);
        Parser parser = new Parser(lexer);
        Program p = parser.ParseProgram();
        Evaluator.Eval(p, env);

        idDropDown.ClearOptions();
        idDropDown.AddOptions(new List<string>(env.store.Keys));
    }

    public void Bind()
    {
        string id = idDropDown.options[idDropDown.value].text;
        string relation = relationDropDown.options[relationDropDown.value].text;
        GameObject gameObject;
        if (objectDropDown.value < bindableObjects.Count)
        {
            gameObject = bindableObjects[objectDropDown.value];
            gameObject.name = "Cube" + ++objectCount[objectDropDown.value];
            Bind(id, gameObject, relation);
        }
        else
        {
            gameObject = GameObject.Find(objectDropDown.options[objectDropDown.value].text);
            Bind(id, gameObject.name, relation);
        }
    }
    void Bind(string id, GameObject gameObject, string relation)
    {
        bounds.Add(id+ " -> " + gameObject.name+ " . " + relation,new KeyValuePair<GameObject, Bound>(Instantiate(gameObject,nextPlace,Quaternion.identity,GameObject.Find("World").transform),new Bound(id,relation)));
        bounds[id + " -> " + gameObject.name + " . " + relation].Key.name = gameObject.name;
        objectDropDown.AddOptions(new List<string> { gameObject.name });
        nextPlace += new Vector3(1f,0f,0f);
        boundDropDown.ClearOptions();
        boundDropDown.AddOptions(new List<string>(bounds.Keys));
    }

    void Bind(string id, string gameObject, string relation)
    {
        foreach (string bound in bounds.Keys)
        {
            string objectName = bound.Split(" -> ")[1].Split(" . ")[0];
            string rel = bound.Split(" -> ")[1].Split(" . ")[1];

            if (gameObject == objectName)
            {
                if (relation == rel || relation.Length == rel.Length + 1)
                {
                    return;
                }
            }
        }
        bounds.Add(id + " -> " + gameObject + " . " + relation, new KeyValuePair<GameObject, Bound>(GameObject.Find(gameObject), new Bound(id, relation)));
        boundDropDown.ClearOptions();
        boundDropDown.AddOptions(new List<string>(bounds.Keys));
    }

    public void Unbind()
    {
        string bound = boundDropDown.options[boundDropDown.value].text;
        bounds.Remove(bound);
        boundDropDown.ClearOptions();
        boundDropDown.AddOptions(new List<string>(bounds.Keys));
    }

    public void Run()
    {
        Lexer lexer = new Lexer(input.text);
        Parser parser = new Parser(lexer);
        Program p = parser.ParseProgram();
        Evaluator.Eval(p, env);
        foreach (var pair in bounds.Values)
        {
            string rel = pair.Value.GetRelation();
            if (rel[0] == 'S')
            {
                Vector3 newScale = new Vector3((rel == "ScaleX" || rel == "Scale") ? 1f : 0f, (rel == "ScaleY" || rel == "Scale") ? 1f : 0f, (rel == "ScaleZ" || rel == "Scale") ? 1f : 0f);
                pair.Key.transform.localScale = newScale * ((Integer)env.store[pair.Value.GetID()]).value / 10;
            }
            if (rel[0] == 'R')
            {
                Vector3 newRot = new Vector3((rel == "RotX" || rel == "Rot") ? 1f : 0f, (rel == "RotY" || rel == "Rot") ? 1f : 0f, (rel == "RotZ" || rel == "Rot") ? 1f : 0f);
                pair.Key.transform.rotation = Quaternion.Euler(newRot * ((Integer)env.store[pair.Value.GetID()]).value);
            }
        }
    }

    class Bound
    {
        string id;
        string relation;

        public string GetRelation()
        {
            return relation;
        }

        public string GetID()
        {
            return id;
        }

        public Bound(string _id, string _relation)
        {
            id = _id;
            relation = _relation;
        }
    }
}
