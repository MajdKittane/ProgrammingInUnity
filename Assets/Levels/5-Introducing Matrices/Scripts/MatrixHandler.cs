using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using System.Threading;

public class MatrixHandler : ArrayHandler
{

    //Matrix Generating Variables
    int numOfLevels;

    //Data Transmitted Between Threads
    (int, int) indexToCheck;

    // Start is called before the first frame update
    public override void Start()
    {
        BaseStart();
        isMatrix = true;
        maxIndex = 0;
        cubeList = CreateCubeMatrix();
        for (int i =0; i<numOfLevels; i++)
        {
            List<MyInterpreter.Object> tempDrop = new();
            for (int j=0; j<numOfEntries; j++)
            {
                tempDrop.Add(falseBool);
            }
            dropList.Add(new Array { elements = tempDrop });
        }
        env.store["cubes"] = new Array { elements = cubeList };
        env.store["drop"] = new Array { elements = dropList };
        env.store["y"] = new Integer { value = numOfLevels };
        env.store["x"] = new Integer { value = numOfEntries };
    }



    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    List<MyInterpreter.Object> CreateCubeMatrix()
    {
        numOfLevels = Random.Range(3, 5);

        List<MyInterpreter.Object> tempCubeList = new List<MyInterpreter.Object>();
        for (int i = 0; i < numOfLevels; i++)
        {
            tempCubeList.Add(new Array { elements = CreateCubeArray(isMatrix, i) });
        }

        return tempCubeList;
    }

    public override void CheckDrop()
    {
        Array matrix = (Array)env.Get("drop");
        Array row = (Array)matrix.elements[indexToCheck.Item1];
        arrayRoot.GetChild(indexToCheck.Item1 * (numOfEntries) + indexToCheck.Item2).gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.red;
        if (((Boolean)row.elements[indexToCheck.Item2]).value == true)
        {
            arrayRoot.GetChild(indexToCheck.Item1 * (numOfEntries) + indexToCheck.Item2).gameObject.AddComponent<Rigidbody>();
            arrayRoot.GetChild(indexToCheck.Item1 * (numOfEntries) + indexToCheck.Item2).GetComponent<Collider>().enabled = true;
        }
    }

    public override void OnProgramEnd()
    {
        for (int i = 0; i < numOfLevels; i++)
        {
            for (int j =0; j<numOfEntries; j++)
            {
                indexToCheck = (i, j);
                checkDrop = true;
                checkedDrop = false;
                Thread.Sleep(500);
            }
        }

        Thread.Sleep(2000);
        isProgramDone = true;
    }
}