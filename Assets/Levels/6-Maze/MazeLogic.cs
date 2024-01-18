using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeLogic : AbstractPuzzle
{
    TriggerManager triggerManager;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        triggerManager = gameObject.GetComponent<TriggerManager>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Action()
    {
        if (!triggerManager.GetActiveTrigger())
        {
            return;
        }

        if (triggerManager.GetActiveTrigger().interactObject.GetComponent<Door>() is Door door)
        {
            door.OpenDoor();
            Destroy(triggerManager.GetActiveTrigger().gameObject);
        }
    }

    public override void CheckResult()
    {
        return;
    }
}
