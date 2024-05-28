using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sensitivityText;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Dropdown selectMicDropdown;
    
    [SerializeField] private Toggle changeHeightToggle;
    [SerializeField] private Toggle alternateNoSpikesToggle;
    
    [SerializeField] private TextMeshProUGUI playerMoveSpeedText;
    [SerializeField] private Slider moveSpeedSlider;
    
    [SerializeField] private TextMeshProUGUI playerAccelerationText;
    [SerializeField] private Slider accelerationSlider;


    [SerializeField] private TMP_InputField spikeLowInput;
    [SerializeField] private TMP_InputField spikeHighInput;

    [SerializeField] private GameObject main;
    [SerializeField] private GameObject options;

    [SerializeField] private AudioMixerGroup mixer;

    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private TMP_InputField floorSizeInput;
    [SerializeField] private TMP_InputField spikePatterInput;



    private void Start()
    {
        LoadMain();
        
        sensitivityText.text = GameManager.microphoneCutoff.ToString("F3");
        sensitivitySlider.value = GameManager.microphoneCutoff;
        
        SetAcceleration(GameManager.playerAcceleration);
        SetMaxSpeed(GameManager.playerMoveSpeed);
        EditLowSpikeBound(GameManager.minMaxSpikes.x.ToString("G3"));
        EditHighSpikeBound(GameManager.minMaxSpikes.y.ToString("G3"));
        
        RandomHeight(GameManager.changeHeight);
        AlternateNoSpikes(GameManager.alternateNoSpikes);
        
        SetFloorSize(((int)floorPrefab.transform.localScale.x).ToString());
        SetSpikePattern(string.Join(',', GameManager.spikePattern));
        Debug.Log(string.Join(',', GameManager.spikePattern));
        
        foreach (string mic in Microphone.devices)
        {
            selectMicDropdown.options.Add(new TMP_Dropdown.OptionData(mic));
        }

        selectMicDropdown.RefreshShownValue();
    }

    public void Mute(bool val)
    {
        mixer.audioMixer.SetFloat("Volume", val ? -80f : 0f);
        mixer.audioMixer.GetFloat("Volume", out float x);
        mixer.audioMixer.GetFloat("Volume (of Master)", out float y);
        Debug.Log(x + " " + y);

    }
    
    public void SetMicSensitivity(float sensitivity)
    {
        sensitivityText.text = sensitivity.ToString("F3");
        GameManager.microphoneCutoff = sensitivity;
    }

    public void SetMicrophone(int mic)
    {
        GameManager.microphoneDevice = mic;
    }

    public void LoadOption()
    {
        main.SetActive(false);
        options.SetActive(true);
    }
    
    public void SetAcceleration(float accel)
    {
        playerAccelerationText.text = accel.ToString("G3");
        GameManager.playerAcceleration = accel;
        accelerationSlider.value = accel;

    }
    
    public void SetMaxSpeed(float speed)
    {
        playerMoveSpeedText.text = speed.ToString("G3");
        GameManager.playerMoveSpeed = speed;
        moveSpeedSlider.value = speed;
    }

    public void EditLowSpikeBound(string n)
    {
        if (int.TryParse(n, out int val))
        {
            if (val <= GameManager.minMaxSpikes.y)
            {
                GameManager.minMaxSpikes.x = val;
                spikeLowInput.text = val.ToString("G3");
                return;
            }
        }
        spikeLowInput.text = "";

    }
    
    public void EditHighSpikeBound(string n)
    {
        if (int.TryParse(n, out int val))
        {
            if (val >= GameManager.minMaxSpikes.x)
            {
                GameManager.minMaxSpikes.y = val;
                spikeHighInput.text = val.ToString("G3");
                return;
            }
        }
        spikeHighInput.text = "";

    }

    public void SetSpikePattern(string input)
    {
        spikePatterInput.text = input;

        if (input == "")
        {
            GameManager.spikePattern = new int[0];
        }
        
        string[] vals = input.Split(',');
        
        int[] numbers = new int[vals.Length];

        for (int i = 0; i < vals.Length; i++)
        {
            if (int.TryParse(vals[i], out int n))
            {
                if (n >= 0)
                {
                    numbers[i] = n;
                    continue;
                }
            }
            GameManager.spikePattern = new int[0];
            return;
        }

        GameManager.spikePattern = numbers;
    }

    public void SetFloorSize(string input)
    {
        if (int.TryParse(input, out int val))
        {
            if (val >= 10)
            {
                floorPrefab.transform.localScale = new Vector3(val, floorPrefab.transform.localScale.y, floorPrefab.transform.localScale.z);
                floorSizeInput.text = val.ToString("G3");
                return;
            }
        }
        floorSizeInput.text = "";
    }

    public void AlternateNoSpikes(bool value)
    {
        GameManager.alternateNoSpikes = value;
        alternateNoSpikesToggle.isOn = value;
    }

    public void RandomHeight(bool value)
    {
        GameManager.changeHeight = value;
        changeHeightToggle.isOn = value;
    }

    public void LoadMain()
    {
        main.SetActive(true);
        options.SetActive(false);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
