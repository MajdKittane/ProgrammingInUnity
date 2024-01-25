using MyInterpreter;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;


public class ArrayHandler : AbstractPuzzle, Observer
{
    [SerializeField] GameObject arrayEntryPrefab;
    [SerializeField] Transform arrayRoot;
    Color[] colors = { Color.red, Color.green, Color.blue };
    int numOfEntries;
    List<MyInterpreter.Object> dropList = new List<MyInterpreter.Object>();
    List<MyInterpreter.Object> cubeList = new List<MyInterpreter.Object>();
    Boolean falseBool = new Boolean { value = false };
    bool running, ran = false;
    bool checkDrop, checkedDrop = false;
    int indexToCheck;
    bool createNextArray, doneCreatingArray = false;
    int currentArrayIndex = 0;
    bool checkingResults, checkedResults = false;
    bool win = true;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        CreateCubeArray(currentArrayIndex);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (levelManager.codeSaved && !ran)
        {
            levelManager.interactText.GetComponent<TMPro.TextMeshProUGUI>().text = "Press F to Run Code";
            levelManager.interactText.SetActive(true);
        }
        else
        {
            levelManager.interactText.SetActive(false);
        }

        if (running && !ran)
        {
            ran = true;
            thread.Start();
        }

        if (checkDrop && !checkedDrop)
        {
            arrayRoot.GetChild(indexToCheck).gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = colors[currentArrayIndex];

            if (((Boolean)((Array)env.Get("drop")).elements[indexToCheck]).value == true)
            {
                arrayRoot.GetChild(indexToCheck).gameObject.AddComponent<Rigidbody>();
                arrayRoot.GetChild(indexToCheck).GetComponent<Collider>().enabled = true;
            }

            checkDrop = false;
            checkedDrop = true;
        }

        if (checkingResults && !checkedResults)
        {
            CheckResult();
            checkedResults = true;
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
            cubeList.Add(new Integer { value = Random.Range(0, 3) });
            entry.GetComponent<MeshRenderer>().material.color = colors[((Integer)cubeList[i]).value];
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

    public void OnProgramEnd()
    {
        for (int i =0; i<numOfEntries; i++)
        {
            indexToCheck = i;
            checkDrop = true;
            checkedDrop = false;
            Thread.Sleep(500);
        }

        checkingResults = true;
        checkedResults = false;
        Thread.Sleep(700);
        currentArrayIndex++;
        createNextArray = true;
        doneCreatingArray = false;
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
        if (currentArrayIndex >= 2 && win)
        {
            levelManager.Win();
            return;
        }
    }
}
