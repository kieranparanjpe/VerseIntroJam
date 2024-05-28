using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI breathsText;
    [SerializeField] private LevelSpawner levelSpawner;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject pause;

    private void Start()
    {
        gameOver.SetActive(false);
        pause.SetActive(false);
    }
    
    public void LoadMenu()
    {
        GameManager.Dead = false;
        GameManager.Score = 0;
        GameManager.BreathsTaken = 0;
        GameManager.Pause = false;

        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        scoreText.text = GameManager.Score.ToString();
        breathsText.text = $"Breaths Taken: {GameManager.BreathsTaken}";

        if (GameManager.Dead)
        {
            gameOver.SetActive(true);
        }
        
        pause.SetActive(GameManager.Pause);
    }

    public void Retry()
    {
        GameManager.Dead = false;
        GameManager.Score = 0;
        GameManager.BreathsTaken = 0;
        GameManager.Pause = false;
        levelSpawner.Initialise();
        gameOver.SetActive(false);
    }
    

    public void UnPause()
    {
        GameManager.Pause = false;
        pause.SetActive(false);
    }
}