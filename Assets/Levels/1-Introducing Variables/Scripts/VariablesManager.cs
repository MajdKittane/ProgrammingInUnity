using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using System.Threading;

public class VariablesManager : AbstractPuzzle, Observer
{
    [SerializeField] Transform spawnTransform;
    [SerializeField] GameObject variableObject;
    [SerializeField] TMPro.TextMeshProUGUI objecitve;
    Vector3 spawnPosition;
    public int[] integers;
    public bool[] integersInstantiated;
    public bool[] booleans;
    public bool[] booleansInstantiated;
    List<string> variablesUsed;
    public int remainingLetUses;
    bool isReady, isRunning = false;
    bool readyToSpawn, spawned = false;
    (MyInterpreter.Object,string) toSpawn;
    public bool isFull = false;
    

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        InstantaiteArrays();

        UpdateObjectiveText();

        remainingLetUses = integers.Length + booleans.Length;
        spawnPosition = spawnTransform.position;
    }



    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (levelManager.codeSaved && !isRunning)
        {
            levelManager.interactText.GetComponent<TMPro.TextMeshProUGUI>().text = "Press F to Run Code";
            levelManager.interactText.SetActive(true);
        }    
        else
        {
            levelManager.interactText.SetActive(false);
        }

        if (isReady && !isRunning)
        {
            isRunning = true;
            variablesUsed = new();
            thread.Start();
        }

        if (readyToSpawn && !spawned)
        {
            readyToSpawn = false;
            spawned = true;
            Debug.LogError("Spawn Called;");
            Spawn();
        }

        if (isFull)
        {
            CheckResult();
        }
    }

    void InstantaiteArrays()
    {
        integers = new int[Random.Range(3, 5)];
        integersInstantiated = new bool[integers.Length];
        booleans = new bool[Random.Range(2, 4)];
        booleansInstantiated = new bool[booleans.Length];
        for (int i = 0; i < integers.Length; i++)
        {
            integers[i] = Random.Range(1, 15);
            integersInstantiated[i] = false;
        }

        for (int i = 0; i < booleans.Length; i++)
        {
            booleans[i] = Random.Range(0, 2) == 1 ? true : false;
            booleansInstantiated[i] = false;
        }
    }

    void UpdateObjectiveText()
    {
        objecitve.text = "Create " + integers.Length + " Integers with values : ";
        for (int i = 0; i < integers.Length; i++)
        {
            objecitve.text = objecitve.text + integers[i];
            if (i != integers.Length - 1)
            {
                objecitve.text = objecitve.text + ",";
            }
            else
            {
                objecitve.text = objecitve.text + ".\n";
            }
        }
        objecitve.text = objecitve.text + "And " + booleans.Length + " Booleans with values : ";
        for (int i = 0; i < booleans.Length; i++)
        {
            objecitve.text = objecitve.text + booleans[i];
            if (i != booleans.Length - 1)
            {
                objecitve.text = objecitve.text + ",";
            }
            else
            {
                objecitve.text = objecitve.text + ".\n";
            }
        }
    }

    public void OnBlockEnd()
    {
        return;
    }

    public void OnLetStatement(string name, MyInterpreter.Object value, List<Integer> indexes)
    {
        Debug.LogWarning("LetSTATEMENT");

        if (isFull)
        {
            return;
        }

        if (value is Integer objI)
        {
            for (int i = 0; i < integers.Length; i++)
            {
                if (objI.value == integers[i] && !integersInstantiated[i])
                {
                    integersInstantiated[i] = true;
                }
            }
            toSpawn = (objI, name + objI.ToString());
            readyToSpawn = true;
            spawned = false;

            Thread.Sleep(500);
        }


        else if (value is Boolean objB)
        {
            for (int i = 0; i < booleans.Length; i++)
            {
                if (objB.value == booleans[i] && !booleansInstantiated[i])
                {
                    booleansInstantiated[i] = true;
                }
            }

            toSpawn = (objB, name + objB.ToString());
            readyToSpawn = true;
            spawned = false;

            Thread.Sleep(500);
        }
        

        remainingLetUses--;

        Thread.Sleep(1000);

        if (remainingLetUses <= 0)
        {
            isFull = true;
        }
    }

    void Spawn()
    {
        Debug.Log("Spawned " + toSpawn);
        if (toSpawn.Item1 is Integer objI)
        {
            GameObject go = Instantiate(variableObject, spawnPosition + new Vector3(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 0.3f), Random.Range(0.2f, 1.0f)), Quaternion.identity);
            go.transform.localScale *= objI.value;
            go.AddComponent<Rigidbody>();
        }
        else if (toSpawn.Item1 is Boolean objB)
        {
            GameObject go = Instantiate(variableObject, spawnPosition + new Vector3(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 0.3f), Random.Range(0.2f, 1.0f)), Quaternion.identity);
            go.transform.localScale *= 2;
            go.GetComponent<MeshRenderer>().material.color = objB.value ? Color.green : Color.red;
            go.AddComponent<Rigidbody>();
        }
        
    }

    public void OnLoopEnd()
    {
        return;
    }

    public void OnLoopIterationEnd()
    {
        return;
    }

    public void OnProgramEnd()
    {
        return;
    }

    public override void Action()
    {
        if (!isReady && levelManager.codeSaved)
        {
            isReady = true;
        }
    }

    public override void CheckResult()
    {
        for (int i = 0; i<integers.Length; i++)
        {
            if (!integersInstantiated[i])
            {
                levelManager.Lose();
                return;
            }
        }

        for (int i = 0; i < booleans.Length; i++)
        {
            if (!booleansInstantiated[i])
            {
                levelManager.Lose();
                return;
            }
        }
        levelManager.Win();
    }
}
