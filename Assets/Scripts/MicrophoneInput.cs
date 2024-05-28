using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
    [SerializeField] private int microphoneSelect = 0;
    [SerializeField] private int clipLength = 1;
    [SerializeField] private int sampleRate = 44100;
    [SerializeField] private int sampleLength = 256; 
    
    private float microphoneCutoffPoint = 0.025f;

    [SerializeField] private bool debug;

    public static bool Begin { get; private set; }
    public static bool Active { get; private set; }
    public static bool End { get; private set; }

    private string microphoneName;
    private string outputFileName;
    private string outputFilePath;
    private float lastMeanAmplitude = 0;

    [SerializeField] private LineRenderer chart;
    private AudioClip microphoneStream;
    private AudioSource audioSource;

    void Start()
    {
        outputFilePath = Path.Combine(Application.dataPath, @"MicrophoneOutput\");
        outputFileName = @"microphoneOutput_" + DateTime.Now.ToString("dd MMM (HH mm ss)") + ".csv";
        SetParams();
        StartMic();
    }

    private void SetParams()
    {
        microphoneSelect = GameManager.microphoneDevice;
        if (microphoneCutoffPoint > 0)
            microphoneCutoffPoint = GameManager.microphoneCutoff;
    }

    private void StartMic()
    {
        microphoneName = Microphone.devices[microphoneSelect];
        Debug.Log("Microphone Options: " + string.Join(',', Microphone.devices));
        microphoneStream = Microphone.Start(microphoneName, true, clipLength, sampleRate);

        StartAudioSource();
    }

    private void StartAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = microphoneStream;
        audioSource.Play();
        audioSource.loop = true;
    }

    void Update()
    {
        float[] audioData = new float[sampleLength];
        microphoneStream.GetData(audioData, Mathf.Abs(Microphone.GetPosition(microphoneName) - sampleLength));
        float mean = computeMean(audioData) * 10;

        if (chart != null)
        {
            int count = chart.positionCount;
            chart.positionCount += 1;
            chart.SetPosition(count, new Vector3(Time.time, mean * 10, 0));
        }

        float[] fft = FFT(64);

        if (mean >= microphoneCutoffPoint)
        {
            Begin = !Active;
            Active = true;
            End = false;
        }
        else
        {
            End = Active;
            Active = false;
            Begin = false;
        }

        if (debug)
            File.AppendAllText(Path.Combine(outputFilePath, outputFileName), 
            $"{Time.time}, {mean}, {string.Join(',', fft)}{Environment.NewLine}");
    }

    private float[] FFT(int size)
    {
        float[] samples = new float[size];
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);

        return samples;
    }

    private float computeMean(float[] arr)
    {
        float sum = 0f;
        foreach (float element in arr)
        {
            sum += Mathf.Abs(element);
        }

        sum /= arr.Length;

        return sum;
    }
}
