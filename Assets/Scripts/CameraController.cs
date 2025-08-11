using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform giocatore;
    public Transform palla;

    public Vector3 posizioneIniziale;
    public Quaternion rotazioneIniziale;

    public Vector3 posizioneBattuta;
    public Quaternion rotazioneBattuta;

    public Vector3 posizioneGioco;
    public Quaternion rotazioneGioco;

    public Vector3 posizionePunteggio;
    public Quaternion rotazionePunteggio;

    public Vector3 posizionePrimaPersona;
    public Quaternion rotazionePrimaPersona;

    [SerializeField] public float velocitaTransizione;

    private Vector3 posizioneTarget;
    private Quaternion rotazioneTarget;

    public GameObject pannelloVersus;
    public TMP_Text botName;
    public TMP_Text gameDaVText;

    public bool inPrimaPersona = false;
    public bool inPunteggio = true;

    void Start()
    {
        posizioneTarget = posizioneIniziale;
        rotazioneTarget = rotazioneIniziale;
        gameDaVText.gameObject.SetActive(false);
        inPunteggio = true;
        inPrimaPersona = false;

        // Imposta il testo per il nome del bot e il numero di game da vincere
        if (PlayerPrefs.GetString("bot-attuale") == "AvvFacile")
        {
            botName.text = "Emanuele";
            gameDaVText.text = "Vinci un game";
        } else if (PlayerPrefs.GetString("bot-attuale") == "AvvMedio")
        {
            botName.text = "Alfonso";
            gameDaVText.text = "Vinci due game";
        }
        else if (PlayerPrefs.GetString("bot-attuale") == "AvvDifficile")
        {
            botName.text = "Tommaso";
            gameDaVText.text = "Vinci tre game";
        }
    }

    void Update()
    {
        if (inPrimaPersona && !inPunteggio && giocatore != null && !pannelloVersus.activeSelf)
        {
            // Aggiorna la posizione e la rotazione della telecamera per seguire il giocatore
            Vector3 posizioneDesiderata = giocatore.position + posizionePrimaPersona;
            Quaternion rotazioneDesiderata = giocatore.rotation * rotazionePrimaPersona;

            transform.position = Vector3.Lerp(transform.position, posizioneDesiderata, Time.deltaTime * velocitaTransizione);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotazioneDesiderata, Time.deltaTime * velocitaTransizione);
        }
        else
        {
            // Transizione della posizione
            transform.position = Vector3.Lerp(transform.position, posizioneTarget, Time.deltaTime * velocitaTransizione);

            // Transizione della rotazione
            transform.rotation = Quaternion.Slerp(transform.rotation, rotazioneTarget, Time.deltaTime * velocitaTransizione);
        }
    }

    public void InquadraVersus()
    {
        pannelloVersus.SetActive(true);
        posizioneTarget = posizioneIniziale;
        rotazioneTarget = rotazioneIniziale;
        inPunteggio = true;
    }

    public void InquadraBattuta()
    {
        pannelloVersus.SetActive(false);
        posizioneTarget = posizioneBattuta;
        rotazioneTarget = rotazioneBattuta;
        inPunteggio = false;
    }

    public void InquadraGioco()
    {
        posizioneTarget = posizioneGioco;
        rotazioneTarget = rotazioneGioco;
        inPunteggio = false;
    }

    public void InquadraPunteggio()
    {
        posizioneTarget = posizionePunteggio;
        rotazioneTarget = rotazionePunteggio;
        inPunteggio = true;
    }

    public void InquadraPrimaPersona()
    {
        Debug.Log("InquadraPrimaPersona");
        if (!inPrimaPersona)
            inPrimaPersona = true;
        else
            inPrimaPersona = false;
    }

}
