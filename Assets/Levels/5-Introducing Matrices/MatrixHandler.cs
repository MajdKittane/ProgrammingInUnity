using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using System.Threading;

public class MatrixHandler : AbstractPuzzle, Observer
{
    [SerializeField] GameObject arrayEntryPrefab;
    [SerializeField] Transform matrixRoot;
    Color[] colors = { Color.red, Color.green, Color.blue };
    int numOfEntries, numOfLevels;
    List<MyInterpreter.Object> cubeList = new List<MyInterpreter.Object>();
    List<MyInterpreter.Object> dropList = new List<MyInterpreter.Object>();
    Boolean falseBool = new Boolean { value = false };
    bool running, ran = false;
    bool checkDrop, checkedDrop = false;
    (int, int) indexToCheck;
    bool checkingResults, checkedResults = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        CreateCubeMatrix();
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
            Array matrix = (Array)env.Get("drop");
            Array row = (Array)matrix.elements[indexToCheck.Item1];
            matrixRoot.GetChild(indexToCheck.Item1 * (numOfEntries) + indexToCheck.Item2).gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.red;
            if (((Boolean)row.elements[indexToCheck.Item2]).value == true)
            {
                matrixRoot.GetChild(indexToCheck.Item1 * (numOfEntries) + indexToCheck.Item2).gameObject.AddComponent<Rigidbody>();
                matrixRoot.GetChild(indexToCheck.Item1 * (numOfEntries) + indexToCheck.Item2).GetComponent<Collider>().enabled = true;
            }
            checkDrop = false;
            checkedDrop = true;
        }

        if (checkingResults && !checkedResults)
        {
            CheckResult();
            checkedResults = true;
        }
    }

    void CreateCubeMatrix()
    {
        numOfEntries = Random.Range(8, 16);
        numOfLevels = Random.Range(3, 5);
        List<MyInterpreter.Object> tempList = new List<MyInterpreter.Object>();
        List<MyInterpreter.Object> tempDropList = new List<MyInterpreter.Object>();
        for (int i = 0; i < numOfLevels; i++)
        {
            for (int j = 0; j < numOfEntries; j++)
            {
                GameObject entry = AddArrayEntry(j, i);
                tempList.Add(new Integer { value = Random.Range(0, 3) });
                entry.GetComponent<MeshRenderer>().material.color = colors[((Integer)tempList[j]).value];
                tempDropList.Add(falseBool);
            }
            cubeList.Add(new Array { elements = tempList });
            dropList.Add(new Array { elements = tempDropList });
            tempList = new List<MyInterpreter.Object>();
            tempDropList = new List<MyInterpreter.Object>();
        }

        env.store["cubes"] = new Array { elements = cubeList };
        env.store["drop"] = new Array { elements = dropList };
        env.store["y"] = new Integer { value = cubeList.Count };
        env.store["x"] = new Integer { value = ((Array)cubeList[0]).elements.Count };

    }

    GameObject AddArrayEntry(int index, int level)
    {
        GameObject entry;
        if (index == 0)
        {
            entry = Instantiate(arrayEntryPrefab, matrixRoot.position + new Vector3(0, -level * 4, 0), Quaternion.identity, matrixRoot);
            entry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.ToString() + "," + index.ToString();
            return entry;
        }
        else if (index % 2 == 1)
        {
            for (int i = matrixRoot.childCount - 1; i > matrixRoot.childCount - 1 - index; i--)
            {
                matrixRoot.GetChild(i).transform.localPosition += new Vector3(3, 0, 0);
            }
        }
        entry = Instantiate(arrayEntryPrefab, matrixRoot.GetChild(matrixRoot.childCount - 1).position - new Vector3(3, 0, 0), Quaternion.identity, matrixRoot);
        entry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.ToString() + "," + index.ToString();
        return entry;
    }

    public void OnLoopIterationEnd()
    {
        return;
    }

    public void OnLetStatement(string name, MyInterpreter.Object value, List<Integer> indexes)
    {
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
        for (int i = 0; i < numOfLevels; i++)
        {
            for (int j = 0; j < numOfEntries; j++)
            {
                indexToCheck = (i,j);
                checkDrop = true;
                checkedDrop = false;
                Thread.Sleep(500);
            }
        }

        checkingResults = true;
        checkedResults = false;
        Thread.Sleep(300);
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
        for (int i = 0; i < matrixRoot.childCount; i++)
        {
            if (matrixRoot.GetChild(i).GetComponent<Rigidbody>() != null && matrixRoot.GetChild(i).GetComponent<MeshRenderer>().material.color != colors[0])
            {
                levelManager.Lose();
                return;
            }

            if (matrixRoot.GetChild(i).GetComponent<Rigidbody>() == null && matrixRoot.GetChild(i).GetComponent<MeshRenderer>().material.color == colors[0])
            {
                levelManager.Lose();
                return;
            }
        }

        levelManager.Win();
    }
}

