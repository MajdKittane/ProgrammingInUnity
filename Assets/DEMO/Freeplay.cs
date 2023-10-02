using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;

public class Freeplay : MonoBehaviour
{
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
        env = new Environment();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckForVariables()
    {
        Environment tempEnv = new Environment();
        Lexer lexer = new Lexer(input.text);
        Parser parser = new Parser(lexer);
        Program p = parser.ParseProgram();
        Evaluator.Eval(p, tempEnv);

        idDropDown.ClearOptions();
        idDropDown.AddOptions(new List<string>(tempEnv.store.Keys));
    }

    public void Bind()
    {
        string id = idDropDown.options[idDropDown.value].text;
        GameObject gameObject = bindableObjects[objectDropDown.value];
        string relation = relationDropDown.options[relationDropDown.value].text;

        Bind(id, gameObject, relation);
    }
    void Bind(string id, GameObject gameObject, string relation)
    {
        bounds.Add(id+ " -> " + gameObject.name+ " . " + relation,new KeyValuePair<GameObject, Bound>(Instantiate(gameObject,nextPlace,Quaternion.identity,GameObject.Find("World").transform),new Bound(id,relation)));
        boundDropDown.ClearOptions();
        boundDropDown.AddOptions(new List<string>(bounds.Keys));
    }

    class Bound
    {
        string id;
        string relation;

        public Bound(string _id, string _relation)
        {
            id = _id;
            relation = _relation;
        }
    }
}
