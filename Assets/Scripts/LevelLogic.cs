using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLogic : MonoBehaviour
{
    [SerializeField] public GameObject mainUI;
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject loseUI;
    [SerializeField] GameObject finishUI;
    [SerializeField] public TMPro.TMP_InputField input;
    [SerializeField] string nextLevel;
    [SerializeField] GameObject HUD;
    [SerializeField] public GameObject interactText;
    [SerializeField] GameObject writeCodeText;
    [HideInInspector] public bool codeSaved = false;
    // Start is called before the first frame update
    void Start()
    {
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
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
}
