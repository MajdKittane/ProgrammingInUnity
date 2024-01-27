using MyInterpreter;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;


public class ArrayHandler : AbstractPuzzle, Observer
{
    [SerializeField] protected GameObject arrayEntryPrefab;
    [SerializeField] protected Transform arrayRoot;

    //Colors Indexing
    protected Color[] colors = { Color.red, Color.green, Color.blue };

    //Array Generating Varibales
    protected bool isMatrix = false;
    protected int numOfEntries;
    protected List<MyInterpreter.Object> dropList = new List<MyInterpreter.Object>();
    protected  List<MyInterpreter.Object> cubeList = new List<MyInterpreter.Object>();
    protected Boolean falseBool = new Boolean { value = false };

    //Transition Between Threads
    protected bool running, ran = false;
    protected bool checkDrop, checkedDrop = false;
    bool createNextArray, doneCreatingArray = false;
    protected bool checkingResults, checkedResults = false;

    //Data Transmitted Between Threads
    int indexToCheck;
    int currentArrayIndex = 0;
    protected int maxIndex = 2;


    // Start is called before the first frame update
    public override void Start()
    {
        BaseStart();
        cubeList = CreateCubeArray(isMatrix);
        env.store["cubes"] = new Array { elements = cubeList };
        env.store["n"] = new Integer { value = currentArrayIndex };
        env.store["c"] = new Integer { value = cubeList.Count };
    }

    // Update is called once per frame
    public override void Update()
    {
        BaseUpdate();

        UpdateHUD();

        HandleStarting();

        HandleDropping();

        HandleNextArrayCreation();
    }

    void UpdateHUD()
    {
        if (levelManager.codeSaved && !ran)
        {
            levelManager.interactText.GetComponent<TMPro.TextMeshProUGUI>().text = "Press F to Run Code";
            levelManager.interactText.SetActive(true);
        }
        else
        {
            levelManager.interactText.SetActive(false);
        }
    }

    void HandleStarting()
    {
        if (running && !ran)
        {
            ran = true;
            FillDropList();
            thread.Start();
        }
    }

    void HandleNextArrayCreation()
    {
        if (createNextArray && !doneCreatingArray && currentArrayIndex <= maxIndex)
        {
            for (int i = 0; i < arrayRoot.childCount; i++)
            {
                Destroy(arrayRoot.GetChild(i).gameObject);
            }
            arrayRoot.DetachChildren();

            cubeList = CreateCubeArray(isMatrix);

            env.store["cubes"] = new Array { elements = cubeList };
            env.store["n"] = new Integer { value = currentArrayIndex };
            env.store["c"] = new Integer { value = cubeList.Count };
            doneCreatingArray = true;
            createNextArray = false;

            dropList = new();
            for (int i = 0; i < numOfEntries; i++)
            {
                dropList.Add(falseBool);
            }
            env.store["drop"] = new Array { elements = dropList };

            thread = new Thread(Run);
            thread.Start();
        }
    }

    void HandleDropping()
    {
        if (checkDrop && !checkedDrop)
        {
            CheckDrop();
            checkDrop = false;
            checkedDrop = true;
        }
    }


    protected List<MyInterpreter.Object> CreateCubeArray(bool isMatrix,int level = 0)
    {
        List<MyInterpreter.Object>  tempCubeList = new List<MyInterpreter.Object>();

        if (!isMatrix || level == 0) numOfEntries = Random.Range(8, 16);
        for (int i = 0; i < numOfEntries; i++)
        {
            GameObject entry = AddArrayEntry(i,isMatrix,level);
            tempCubeList.Add(new Integer { value = Random.Range(0, 3) });
            entry.GetComponent<MeshRenderer>().material.color = colors[((Integer)tempCubeList[i]).value];
        }

        return tempCubeList;
    }

    GameObject AddArrayEntry(int index, bool isMatrix, int level = 0)
    {
        GameObject entry;
        if (index == 0)
        {
            entry = Instantiate(arrayEntryPrefab, arrayRoot.position + new Vector3(0, -level * 4, 0), Quaternion.identity, arrayRoot);
            entry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = isMatrix ? level.ToString() + "," + index.ToString() : index.ToString();
            return entry;
        }
        else if (index % 2 == 1)
        {
            for (int i = arrayRoot.childCount - 1; i > arrayRoot.childCount - 1 - index; i--)
            {
                arrayRoot.GetChild(i).transform.localPosition += new Vector3(3, 0, 0);
            }
        }
        entry = Instantiate(arrayEntryPrefab, arrayRoot.GetChild(arrayRoot.childCount - 1).position - new Vector3(3, 0, 0), Quaternion.identity, arrayRoot);
        entry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = isMatrix ? level.ToString() + "," + index.ToString() : index.ToString();
        return entry;
    }

    public virtual void FillDropList()
    {
        for (int i = 0; i < numOfEntries; i++)
        {
            dropList.Add(falseBool);
        }
        env.store["drop"] = new Array { elements = dropList };
    }

    public virtual void CheckDrop()
    {
        arrayRoot.GetChild(indexToCheck).gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = colors[currentArrayIndex];
        if (((Boolean)((Array)env.Get("drop")).elements[indexToCheck]).value == true)
        {
            arrayRoot.GetChild(indexToCheck).gameObject.AddComponent<Rigidbody>();
            arrayRoot.GetChild(indexToCheck).GetComponent<Collider>().enabled = true;
        }
    }

    public void OnLoopIterationEnd()
    {
        return;
    }

    public void OnLetStatement(string name, MyInterpreter.Object value, List<Integer> indexes)
    {
        return;
    }

    public void OnBlockEnd()
    {
        return;
    }

    public void OnLoopEnd()
    {
        return;
    }

    public virtual void OnProgramEnd()
    {
        for (int i =0; i<numOfEntries; i++)
        {
            indexToCheck = i;
            checkDrop = true;
            checkedDrop = false;
            Thread.Sleep(500);
        }

        Thread.Sleep(1000);
        isProgramDone = true;
    }

    public override void Action()
    {
        if (!running && levelManager.codeSaved)
        {
            running = true;
        }
    }

    public override void CheckResult()
    {
        for (int i = 0; i < arrayRoot.childCount; i++)
        {
            if (arrayRoot.GetChild(i).GetComponent<Rigidbody>() != null && arrayRoot.GetChild(i).GetComponent<MeshRenderer>().material.color != colors[currentArrayIndex])
            {
                levelManager.Lose();
                return;
            }

            if (arrayRoot.GetChild(i).GetComponent<Rigidbody>() == null && arrayRoot.GetChild(i).GetComponent<MeshRenderer>().material.color == colors[currentArrayIndex])
            {
                levelManager.Lose();
                return;
            }
        }

        if (currentArrayIndex >= maxIndex)
        {
            levelManager.Win();
            return;
        }

        currentArrayIndex++;
        isProgramDone = false;
        createNextArray = true;
        doneCreatingArray = false;
    }

    public void HandleOutputStream(string str)
    {
        return;
    }
}
