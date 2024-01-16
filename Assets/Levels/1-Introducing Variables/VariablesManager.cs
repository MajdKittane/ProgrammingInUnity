using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using System.Threading;

public class VariablesManager : IPuzzleLogic, Observer
{
    [SerializeField] Transform spawnTransform;
    [SerializeField] GameObject variableObject;
    [SerializeField] TMPro.TextMeshProUGUI objecitve;
    Vector3 spawnPosition;
    int[] integers;
    bool[] integersInstantiated;
    bool[] booleans;
    bool[] booleansInstantiated;
    int remainingLetUses;
    bool isReady, isRunning = false;
    bool readyToSpawn, spawned = false;
    bool isFull = false;
    

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        InstantaiteArrays();

        UpdateObjectiveText();

        remainingLetUses = integers.Length + booleans.Length;
        spawnPosition = spawnTransform.position;
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

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (isReady && !isRunning)
        {
            isRunning = true;
            thread.Start();
        }

        if (readyToSpawn && !spawned)
        {
            Spawn();
            readyToSpawn = false;
            spawned = true;
        }

        if (isFull)
        {
            CheckResult();
        }
    }

    public void OnBlockEnd()
    {
        return;
    }

    public void OnLetStatement()
    {
        if (remainingLetUses <= 0)
        {
            isFull = true;
            return;
        }

        readyToSpawn = true;
        spawned = false;
        remainingLetUses--;
        Thread.Sleep(1000);
    }

    void Spawn()
    {
        foreach (MyInterpreter.Object obj in env.store.Values)
        {
            for (int i = 0; i < integers.Length; i++)
            {
                if (obj is Integer objI)
                {
                    GameObject go = Instantiate(variableObject, spawnPosition + new Vector3(Random.Range(0.2f,1.0f), Random.Range(0.2f, 0.3f), Random.Range(0.2f, 1.0f)), Quaternion.identity);
                    go.transform.localScale *= objI.value;
                    go.AddComponent<Rigidbody>();
                    if (objI.value == integers[i] && !integersInstantiated[i])
                    {
                        integersInstantiated[i] = true;
                    }
                    return;
                }
            }

            for (int i = 0; i < booleans.Length; i++)
            {
                if (obj is Boolean objB)
                {
                    GameObject go = Instantiate(variableObject, spawnPosition + new Vector3(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 0.3f), Random.Range(0.2f, 1.0f)), Quaternion.identity);
                    go.GetComponent<MeshRenderer>().material.color = objB.value ? Color.green : Color.red;
                    go.transform.localScale *= 2;
                    go.AddComponent<Rigidbody>();
                    
                    if (objB.value == booleans[i] && !booleansInstantiated[i])
                    {
                        booleansInstantiated[i] = true;
                    }
                    return;
                }
            }
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
            if (!integersInstantiated[i]) levelManager.Lose();
            return;
        }

        for (int i = 0; i < booleans.Length; i++)
        {
            if (!booleansInstantiated[i]) levelManager.Lose();
            return;
        }

        levelManager.Win();
    }
}
