using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerManager : MonoBehaviour
{
    [SerializeField] GameObject interactText;
    [SerializeField] public GameObject fullScreenText;
    [SerializeField] public GameObject inputField;
    [SerializeField] public Button inputButton;
    Dictionary<InteractTrigger,bool> interactTriggers = new Dictionary<InteractTrigger, bool>();
    public Dictionary<InteractTrigger, bool> interactDone = new Dictionary<InteractTrigger, bool>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.LogWarning(GetActiveTrigger());

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
        interactDone[trigger] = false;
    }

    public InteractTrigger GetActiveTrigger()
    {
        
        foreach (InteractTrigger trigger in interactTriggers.Keys)
        {
            Debug.LogWarning(trigger + "  " + interactTriggers[trigger]);
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
        Debug.LogWarning(trigger + "  " + state);
    }
}
