using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundGen : MonoBehaviour
{
    /* 
    // edited by Mateusz Hobot
    intially mentioned here: https://www.youtube.com/watch?v=GqHFGMy_51c and adjusted it for our purposes
     */
    float sampleRate;
    AudioSource audioSource;
    GameObject keyboard;

    public Dictionary<int, List<float>> frequencies;
   

    List<float> phase, increment; 
    
    void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;
        phase = new List<float>{0,0};
        increment = new List<float>{0,0};
        frequencies = new Dictionary<int, List<float>>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = true;

        // Add low pass filter and set cutoff frequency
        AudioLowPassFilter lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
       lowPassFilter.cutoffFrequency = 1000f;
    }

    public void OnKey(int keyNumber){
        float freq = 440 * Mathf.Pow(2, ((float)keyNumber-69f)/12f); 
        frequencies[keyNumber] = new List<float>{freq, 0};
    }

    public void changePitch(int keyNumber, float pitch){
        try{
        frequencies[keyNumber][1] = pitch;

        } catch{
            Debug.Log("Not yet here");
        }
    }

    public void onKeyOff(int keyNumber){
        frequencies.Remove(keyNumber);
    }
   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    
   
   void OnAudioFilterRead(float[] data, int channels)
{
    try
    {
        int counter = 0;
        foreach (var item in frequencies.Keys)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                float freq = frequencies[item][0];
                float vibratoAmount = frequencies[item][1];
                float incrementAmount = (freq + freq / 2 * vibratoAmount) * 2f * Mathf.PI / sampleRate;
                phase[counter] += incrementAmount;
                float envelope = Mathf.Clamp01(1f - (float)i / (data.Length / 2));
                data[i] += (float)(Mathf.Sin(phase[counter]) * envelope);
                if (phase[counter] > (Mathf.PI * 2f))
                {
                    phase[counter] = 0f;
                }
            }
            counter++;
        }
    }
    catch (Exception e)
    {
        Debug.Log("Error occurred while accessing frequency: " + e.Message);
    }
}

}