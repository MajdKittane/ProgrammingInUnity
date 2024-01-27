using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLogic : MonoBehaviour
{
    [SerializeField] private List<GuideData> turorials;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button saveCodeButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject finishUI;
    [SerializeField] private GameObject guideUI;
    [SerializeField] private GameObject HUD;
    [SerializeField] private TMPro.TMP_InputField input;
    [SerializeField] private GameObject interactText;
    [HideInInspector] public bool codeSaved { get; private set; } = false;
    [SerializeField] private string nextLevel;
    [SerializeField] private GameObject writeCodeText;
    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(() => HideMainUI());
        saveCodeButton.onClick.AddListener(() => SaveCode());
        helpButton.onClick.AddListener(() => ShowGuide());
        guideUI.GetComponent<Guide>().SetTutorials(turorials);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowMainUI();
        }

        if (codeSaved)
        {
            writeCodeText.SetActive(false);
        }
        else
        {
            writeCodeText.SetActive(true);
        }

        if (!guideUI.activeSelf && Time.timeScale == 0f && !finishUI.activeSelf)
        {
            mainUI.SetActive(true);
        }
    }

    public void Pause()
    {
        HUD.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        HUD.SetActive(true);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void ShowMainUI()
    {
        if (!mainUI.activeSelf && !codeSaved && Time.timeScale != 0f)
        {
            mainUI.SetActive(true);
            Pause();
        }
    }

    public void HideMainUI()
    {
        mainUI.SetActive(false);
        Resume();
    }

    public void SaveCode()
    {
        HideMainUI();
        codeSaved = true;
    }

    public void Win()
    {
        Pause();
        finishUI.SetActive(true);
        winUI.SetActive(true);
        loseUI.SetActive(false);
    }

    public void Lose()
    {
        Pause();
        finishUI.SetActive(true);
        loseUI.SetActive(true);
        winUI.SetActive(false);
    }

    public void ShowGuide()
    {
        mainUI.SetActive(false);
        guideUI.SetActive(true);
        Pause();
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }

    public void ResetCode()
    {
        codeSaved = false;
    }

    public GameObject GetMainUI()
    {
        return mainUI;
    }
    public GameObject GetInteractText()
    {
        return interactText;
    }
    public TMPro.TMP_InputField GetInput()
    {
        return input;
    }
}
