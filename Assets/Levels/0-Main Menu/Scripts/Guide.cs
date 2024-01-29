using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyInterpreter;

public class Guide : MonoBehaviour, Observer
{
    [SerializeField] List<GuideData> tutorials;
    [SerializeField] TMPro.TextMeshProUGUI codeText;
    [SerializeField] TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] TMPro.TextMeshProUGUI outputText;
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;
    [SerializeField] Button runButton;
    [SerializeField] private Button backButton;
    int currentIndex = 0;
    bool changed = true;
    // Start is called before the first frame update
    
    void Awake()
    {
        nextButton.onClick.AddListener(() => NextTutorial());
        previousButton.onClick.AddListener(() => PreviousTutorial());
        backButton.onClick.AddListener(() => gameObject.SetActive(false));
        runButton.onClick.AddListener(() => ExecuteCode());
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentIndex + 1 >= tutorials.Count)
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

        if (changed && tutorials != null)
        {
            codeText.text = tutorials[currentIndex].codeText;
            descriptionText.text = tutorials[currentIndex].descriptionText;
            outputText.text = "Output:\n";
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

    void ExecuteCode()
    {
        outputText.text = "Output:\n";
        Lexer lexer = new Lexer(tutorials[currentIndex].code);
        Parser parser = new Parser(lexer);
        Program prog = parser.ParseProgram();
        Environment env = new Environment();
        Evaluator.Eval(prog,env,this);
    }

    public void OnLoopIterationEnd()
    {
        return;
    }

    public void OnLoopEnd()
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

    public void OnProgramEnd()
    {
        return;
    }

    public void HandleOutputStream(string str)
    {
        outputText.text += str + " ";
    }

    public void SetTutorials(List<GuideData> data)
    {
        tutorials = data;
    }

    public Button GetBackButton()
    {
        return backButton;
    }
}
