using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musica : MonoBehaviour
{
    public static Musica instance;
    private AudioSource audioSource;
    string inRiproduzione = "traccia-menu";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void CambiaTraccia(string nomeTraccia)
    {
        if (inRiproduzione == nomeTraccia) return;

        AudioClip nuovaTraccia = Resources.Load<AudioClip>(nomeTraccia);
        if (nuovaTraccia != null && audioSource.clip != nuovaTraccia)
        {
            audioSource.clip = nuovaTraccia;
            audioSource.Play();
            inRiproduzione = nomeTraccia;
        }
        else
        {
            Debug.LogWarning("Traccia non trovata: " + nomeTraccia);
        }
    }

    public void MutaAudio(bool muta)
    {
        audioSource.mute = muta;
    }
}
