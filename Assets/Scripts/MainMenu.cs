using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public Texture2D cursorTexture;
    public bool isMenuPaused{get; private set;}

    private bool isGameOver;

    void Start()
    {
        Time.timeScale = 1f;
        Vector2 hotSpot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) PauseMenu();

        if(BulletManager.Instance.houseList.Count <= 0 && !isGameOver){

            Time.timeScale = 0f;
            gameOverPanel.SetActive(true);
            isGameOver = true;
        }
    }
    public void PauseMenu()
    {
        if(pauseMenuPanel.activeInHierarchy){

            isMenuPaused = false;
            Time.timeScale = 1f;
            pauseMenuPanel.SetActive(false);
        }
        else{

            isMenuPaused = true;
            Time.timeScale = 0f;
            pauseMenuPanel.SetActive(true);
        }
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
    public void RestartButton()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGameButton()
    {
        Application.Quit();
    }
}

