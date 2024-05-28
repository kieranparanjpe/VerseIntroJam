using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int BreathsTaken;
    public static int Score;
    public static bool Dead;
    public static bool Pause;

    public static GameManager singleton;
    
    public static bool changeHeight = true;
    public static bool alternateNoSpikes = false;
    public static Vector2 minMaxSpikes = new Vector2(3, 5);
    public static float playerMoveSpeed = 7;
    public static float playerAcceleration = 500;
    public static int[] spikePattern = new int[0];
    
    public static float microphoneCutoff = 0.025f;
    public static int microphoneDevice;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        if (!Dead && !Pause)
        {
            Score += 1;
        }
    }

    private void Update()
    {
        if (!Dead && !Pause)
        {
            if (MicrophoneInput.Begin)
                BreathsTaken++;
        }

        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && !Dead)
        {
            Pause = !Pause;
        }
    }
}
