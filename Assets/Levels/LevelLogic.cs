using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLogic : MonoBehaviour
{
    [SerializeField] GameObject mainUI;
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject loseUI;
    [SerializeField] GameObject finishUI;
    [SerializeField] public TMPro.TextMeshProUGUI input;
    [SerializeField] string nextLevel;
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
    }

    void Pause()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Resume()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void ShowMainUI()
    {
        if (!mainUI.activeSelf && !codeSaved)
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
