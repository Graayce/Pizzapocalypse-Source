using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // References
    private GameObject pauseMenu;
    private Animator pauseAnimations;

    // States
    private bool paused;

    // Getters
    public bool Paused { get { return paused; } }

    //Singleton
    public static Menu instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        pauseMenu = GameObject.Find("Canvas").transform.Find("Menu").gameObject;
        pauseAnimations = pauseMenu.GetComponent<Animator>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Main Menu") || DeliveryManager.instance.Failed) { return; }

        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    public void Pause()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        paused = pauseMenu.activeSelf;

        if (paused)
        {
            Time.timeScale = 0;
            pauseAnimations.Play("PauseMenu_Open");
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }

        Cursor.visible = paused;
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Settings()
    {
        // No settings yet :(
    }

    public void Quit()
    {
        Application.Quit();
    }
}
