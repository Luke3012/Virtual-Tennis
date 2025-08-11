using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;

public class Palla : MonoBehaviour
{
    Vector3 posizioneIniziale;
    public string colpitaDa;

    public bool inGioco = true;

    GestionePunteggio gestionePunteggio;
    CameraController cameraController;

    public int rimbalzi = 0;
    private float tempoATerra = 0f;
    private float tempoMassimoATerra = 1.5f; // Tempo massimo che la palla può rimanere a terra

    string pallaAttraversa = "giocatore";
    public AudioClip suonoRimbalzo;
    private AudioSource audioSource;

    void Start()
    {
        posizioneIniziale = transform.position;
        gestionePunteggio = FindObjectOfType<GestionePunteggio>();
        cameraController = FindObjectOfType<CameraController>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ImpostaPosizioneBattuta(Vector3 posizioneBattuta)
    {
        inGioco = false;
        transform.position = posizioneBattuta;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        //La palla fluttua in aria, senza cadere a terra
        GetComponent<Rigidbody>().useGravity = false;
        rimbalzi = 0;
        tempoATerra = 0f;

        // Inquadra il giocatore durante la battuta
        if (cameraController != null) cameraController.InquadraBattuta();
    }

    void Update()
    {
        if (!inGioco || cameraController.inPunteggio || cameraController.pannelloVersus.activeSelf) return;

        
        if (Mathf.Abs(GetComponent<Rigidbody>().velocity.y) < 0.1f)
        {
            tempoATerra += Time.deltaTime;
            if (tempoATerra >= tempoMassimoATerra)
            {
                Debug.Log("La palla è stata a terra troppo tempo!");
                TroppiRimbalzi("La palla ha avuto qualche problemino...");
            }
        }
        else
        {
            tempoATerra = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Se la palla collide con il suolo, incrementa il contatore dei rimbalzi
        if (collision.gameObject.CompareTag("Campo") && inGioco && !cameraController.inPunteggio && !cameraController.pannelloVersus.activeSelf)
        {
            rimbalzi++;
            if (rimbalzi > 1)
            {
                Debug.Log("La palla è rimbalzata due volte!");
                TroppiRimbalzi();
            }
            else
            {
                // Riproduci il suono del rimbalzo
                if (suonoRimbalzo != null)
                {
                    audioSource.PlayOneShot(suonoRimbalzo);
                }
            }
        }
        else
        {
            rimbalzi = 0;
        }

        // Se la palla collide con un muro, riportala alla posizione iniziale
        if (collision.gameObject.CompareTag("Muro") || collision.gameObject.CompareTag("Fuoricampo") || collision.gameObject.CompareTag("Rete"))
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            if (inGioco)
            {
                inGioco = false;

                if (pallaAttraversa == colpitaDa) //potenzialmente potrebbe aver mancato la palla
                    colpitaDa += "fuoricampo";
                Debug.Log("[OnCollisionEnter] La palla è fuoricampo qui: " + collision.gameObject.tag);
                
                gestionePunteggio.AggiungiPunto(colpitaDa, GetMotivationalMessage());
            }
        }
    }

    private void TroppiRimbalzi(string motivo = "Doppio rimbalzo")
    {
        rimbalzi = 0;
        tempoATerra = 0f;

        if (!inGioco) return;
        inGioco = false;
        string avversario = colpitaDa == "giocatore" ? "bot" : "giocatore";
        gestionePunteggio.AggiungiPunto(avversario, motivo);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Fuoricampo") || other.CompareTag("Rete")) && inGioco)
        {
            inGioco = false;
            Debug.Log("[OnTriggerEnter] La palla è qui: " + other.name);

            if (pallaAttraversa == colpitaDa) //potenzialmente potrebbe aver mancato la palla
                colpitaDa += "fuoricampo";
            gestionePunteggio.AggiungiPunto(colpitaDa, GetMotivationalMessage());
        } else if (other.CompareTag("Giocatore") && inGioco)
        {
            Debug.Log("La palla ha attraversato il giocatore! La colpirà?");
            pallaAttraversa = "giocatore";
        } else if (other.CompareTag("Bot") && inGioco)
        {
            Debug.Log("La palla ha attraversato il bot! La colpirà?");
            pallaAttraversa = "bot";
        }
    }
    public string GetMotivationalMessage()
    {
        string[] motivationalMessages;

        if (colpitaDa == "giocatore" || colpitaDa == "botfuoricampo")
        {
            motivationalMessages = new string[]
            {
                "Forza! Puoi farcela!",
                "Non arrenderti, continua a segnare!",
                "Sei un campione, continua cosi'!",
                "Non mollare, il successo e' dietro l'angolo!",
                "Sei imbattibile, continua a segnare punti!"
            };
        }
        else
        {
            motivationalMessages = new string[]
            {
                "Prova ancora!",
                "Non mollare!",
                "Ricorda il tuo obiettivo!",
                "Fallire fa parte del processo!",
                "Ogni errore e' un'opportunita'!",
                "Impara dagli errori!",
                "Sii resiliente!",
                "Continua a provare!",
                "Il successo arrivera'!",
                "Non arrenderti!"
            };
        }

        int randomIndex = UnityEngine.Random.Range(0, motivationalMessages.Length);
        return motivationalMessages[randomIndex];
    }
}
