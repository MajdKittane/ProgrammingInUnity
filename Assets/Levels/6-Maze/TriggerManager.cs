using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    [SerializeField] GameObject interactText;
    Dictionary<InteractTrigger,bool> interactTriggers = new Dictionary<InteractTrigger, bool>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GetActiveTrigger())
        {
            interactText.SetActive(true);
        }

        if (!GetActiveTrigger())
        {
            interactText.SetActive(false);
        }
    }

    public void RegisterInteractTrigger(InteractTrigger trigger)
    {
        interactTriggers[trigger] = false;
    }

    public InteractTrigger GetActiveTrigger()
    {
        foreach (InteractTrigger trigger in interactTriggers.Keys)
        {
            if (interactTriggers[trigger])
            {
                return trigger;
            }
        }
        return null;
    }
    public void OnInteractTrigger(InteractTrigger trigger,bool state)
    {
        interactTriggers[trigger] = state;
    }
}
