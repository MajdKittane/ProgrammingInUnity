using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using System.Threading;

public class MatrixHandler : MonoBehaviour,Observer
{
    [SerializeField] bool useTestInput;
    [TextArea(3, 10)] [SerializeField] string testInput;
    [SerializeField] GameObject arrayEntryPrefab;
    [SerializeField] Transform matrixRoot;
    [SerializeField] MaterialIndexing materials;
    [SerializeField] TMPro.TextMeshProUGUI input;
    int numOfEntries, numOfLevels;
    Environment env;
    List<MyInterpreter.Object> cubeList = new List<MyInterpreter.Object>();
    List<MyInterpreter.Object> dropList = new List<MyInterpreter.Object>();
    Boolean falseBool = new Boolean { value = false };
    Thread thread;
    bool running, ran = false;
    bool checkDrop, checkedDrop = false;
    bool createNextArray, doneCreatingArray = false;
    int currentArrayIndex = 0;
    bool startCounting, doneCounting = false;

    // Start is called before the first frame update
    void Start()
    {
        thread = new Thread(Run);
        env = new Environment();
        CreateCubeMatrix();
    }

    // Update is called once per frame
    void Update()
    {
        if (startCounting && !doneCounting)
        {
            Array arr = (Array)env.Get("cubes");
            for (int i = 0; i<numOfLevels; i++)
            {
                Array arr2 = (Array)arr.elements[i];
                for (int j = 0; j<numOfEntries; j++)
                {
                    //Debug.LogWarning("out(cubes[" + i + "]" + "[" + j + "]);");
                    Lexer l = new Lexer("out(cubes[" + i + "]" + "[" + j + "]);");
                    Program pr = new Parser(l).ParseProgram();
                    Evaluator.Eval(pr,env);
                }
            }
            doneCounting = true;
        }

        if (!running && Input.GetKeyDown(KeyCode.F)) running = true;
        if (running && !ran)
        {
            ran = true;
            thread.Start();
        }

        if (checkDrop && !checkedDrop)
        {
            Array arr = (Array)env.Get("drop");
            for (int i = 0; i < numOfLevels; i++)
            {
                Array arr2 = (Array)arr.elements[i];
                for (int j = 0; j < numOfEntries; j++)
                {
                    if (((Boolean)arr2.elements[j]).value == true && matrixRoot.GetChild(i * (numOfEntries) + j).gameObject.GetComponent<Rigidbody>() == null)
                    {
                        Debug.LogError(i * (numOfEntries) + j);
                        matrixRoot.GetChild(i * (numOfEntries) + j).gameObject.AddComponent<Rigidbody>();
                        matrixRoot.GetChild(i * (numOfEntries) + j).GetComponent<Collider>().enabled = true;
                    }
                }
            }
            checkDrop = false;
            checkedDrop = true;
        }
    }

    void Run()
    {
        Lexer lexer = new Lexer(useTestInput ? testInput : input.text);
        Program prog = new Parser(lexer).ParseProgram();
        Evaluator.Eval(prog, env, this);
    }

    void CreateCubeMatrix()
    {
        numOfEntries = Random.Range(8, 16);
        numOfLevels = Random.Range(3, 5);
        List<MyInterpreter.Object> tempList = new List<MyInterpreter.Object>();
        List<MyInterpreter.Object> tempDropList = new List<MyInterpreter.Object>();
        for (int i = 0; i<numOfLevels; i++)
        {
            for (int j = 0; j < numOfEntries; j++)
            {
                GameObject entry = AddArrayEntry(j,i);
                tempList.Add(new Integer { value = Random.Range(0, materials.materials.Length) });
                entry.GetComponent<MeshRenderer>().material = materials.materials[((Integer)tempList[j]).value];
                tempDropList.Add(new Boolean { value = false });
            }
            cubeList.Add(new Array { elements = tempList });
            dropList.Add(new Array { elements = tempDropList });
            tempList = new List<MyInterpreter.Object>();
            tempDropList = new List<MyInterpreter.Object>();
        }
        
        env.store["cubes"] = new Array { elements = cubeList };
        env.store["drop"] = new Array {elements = dropList };
        env.store["y"] = new Integer { value = cubeList.Count };
        env.store["x"] = new Integer { value = ((Array)cubeList[0]).elements.Count };

        //startCounting = true;
    }

    GameObject AddArrayEntry(int index,int level)
    {
        GameObject entry;
        if (index == 0)
        {
            entry = Instantiate(arrayEntryPrefab,matrixRoot.position + new Vector3(0, -level*4, 0), Quaternion.identity, matrixRoot);
            entry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.ToString() + "," + index.ToString();
            return entry;
        }
        else if (index % 2 == 1)
        {
            for (int i = matrixRoot.childCount - 1; i > matrixRoot.childCount - 1 - index; i--)
            {
                matrixRoot.GetChild(i).transform.localPosition += new Vector3(2,0, 0);
            }
        }
        entry = Instantiate(arrayEntryPrefab, matrixRoot.GetChild(matrixRoot.childCount-1).position - new Vector3(2,0, 0), Quaternion.identity, matrixRoot);
        entry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.ToString() + "," + index.ToString();
        return entry;
    }

    public void OnLoopIterationEnd()
    {
        return;
    }

    public void OnLetStatement()
    {
        checkDrop = true;
        checkedDrop = false;
        Thread.Sleep(200);
    }

    public void OnBlockEnd()
    {
        return;
    }

    public void OnLoopEnd()
    {
        return;
    }

    public void OnProgramEnd()
    {
        return;
    }
}
