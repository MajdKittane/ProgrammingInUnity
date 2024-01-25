using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    [SerializeField] GuideData[] tutorials;
    [SerializeField] TMPro.TextMeshProUGUI codeText;
    [SerializeField] TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;
    [SerializeField] Button backButton;
    int currentIndex = 0;
    bool changed = true;
    // Start is called before the first frame update
    void Start()
    {
        nextButton.onClick.AddListener(() => NextTutorial());
        previousButton.onClick.AddListener(() => PreviousTutorial());
        backButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {
        if (currentIndex + 1 >= tutorials.Length)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
        }

        if (currentIndex - 1 < 0)
        {
            previousButton.interactable = false;
        }
        else
        {
            previousButton.interactable = true;
        }

        if (changed)
        {
            codeText.text = tutorials[currentIndex].codeText;
            descriptionText.text = tutorials[currentIndex].descriptionText;
            changed = false;
        }
    }

    void NextTutorial()
    {
        currentIndex++;
        changed = true;
    }
    
    void PreviousTutorial()
    {
        currentIndex--;
        changed = true;
    }
}
