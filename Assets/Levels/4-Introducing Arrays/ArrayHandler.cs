using MyInterpreter;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;


public class ArrayHandler : MonoBehaviour, Observer
{
    [SerializeField] bool useTestInput;
    [TextArea(3, 10)] [SerializeField] string testInput;
    [SerializeField] GameObject arrayEntryPrefab;
    [SerializeField] Transform arrayRoot;
    [SerializeField] MaterialIndexing materials;
    [SerializeField] TMPro.TextMeshProUGUI input;
    int numOfEntries;
    Environment env;
    List<MyInterpreter.Object> dropList = new List<MyInterpreter.Object>();
    List<MyInterpreter.Object> cubeList = new List<MyInterpreter.Object>();
    Boolean falseBool = new Boolean { value = false };
    Thread thread;
    bool running, ran = false;
    bool checkDrop, checkedDrop = false;
    bool createNextArray, doneCreatingArray = false;
    int currentArrayIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        thread = new Thread(Run);
        env = new Environment();
        CreateCubeArray(currentArrayIndex);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.LogWarning(env.Get("drop[0]"));
        if (!running && Input.GetKeyDown(KeyCode.F)) running = true;
        if (running && !ran)
        {
            ran = true;
            thread.Start(); 
        }

        if (checkDrop && !checkedDrop)
        {
            for (int i = 0; i < numOfEntries; i++)
            {
                if (((Boolean)((Array)env.Get("drop")).elements[i]).value == true && arrayRoot.GetChild(i).GetComponent<Rigidbody>() == null)
                {
                    arrayRoot.GetChild(i).gameObject.AddComponent<Rigidbody>();
                    arrayRoot.GetChild(i).GetComponent<Collider>().enabled = true;
                }
            }
            checkDrop = false;
            checkedDrop = true;
        }

        if (createNextArray && !doneCreatingArray && currentArrayIndex < 3)
        {
            CreateCubeArray(currentArrayIndex);
            doneCreatingArray = true;
            createNextArray = false;
            
        }
    }


    void CreateCubeArray(int n)
    {
        if (n != 0)
        {
            cubeList = new List<MyInterpreter.Object>();
            dropList = new List<MyInterpreter.Object>();
            for (int i = 0; i < arrayRoot.childCount; i++)
            {
                Destroy(arrayRoot.GetChild(i).gameObject);
            }
            arrayRoot.DetachChildren();
        }

        numOfEntries = Random.Range(8, 16);
        for (int i = 0; i < numOfEntries; i++)
        {
            GameObject entry = AddArrayEntry(i);
            cubeList.Add(new Integer { value = Random.Range(0, materials.materials.Length) });
            entry.GetComponent<MeshRenderer>().material = materials.materials[((Integer)cubeList[i]).value];
            dropList.Add(falseBool);
        }
        env.store["cubes"] = new Array { elements = cubeList };
        env.store["drop"] = new Array { elements = dropList };
        env.store["n"] = new Integer { value = n };
        env.store["c"] = new Integer { value = cubeList.Count };

        if (n != 0)
        {
            thread = new Thread(Run);
            thread.Start();
        }
    }

    GameObject AddArrayEntry(int index)
    {
        GameObject entry;
        if (index == 0)
        {
            entry = Instantiate(arrayEntryPrefab, arrayRoot);
            entry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 0.ToString();
            return entry;
        }
        else if (index % 2 == 1)
        {
            for (int i = 0; i < index; i++)
            {
                arrayRoot.GetChild(i).transform.localPosition += new Vector3(2, 0, 0);
            }
        }
        entry = Instantiate(arrayEntryPrefab, arrayRoot.GetChild(index - 1).position - new Vector3(2, 0, 0), Quaternion.identity, arrayRoot);
        entry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = index.ToString();
        return entry;
    }


    void Run()
    {
        Lexer lexer = new Lexer(useTestInput ? testInput : input.text);
        Program prog = new Parser(lexer).ParseProgram();
        Evaluator.Eval(prog, env, this);
    }

    public void OnLoopIterationEnd()
    {
        checkDrop = true;
        checkedDrop = false;
        Thread.Sleep(200);
    }

    public void OnLetStatement()
    {
        return;
    }

    public void OnBlockEnd()
    {
        return;
    }

    public void OnLoopEnd()
    {
        currentArrayIndex++;
        createNextArray = true;
        doneCreatingArray = false;
    }
}
