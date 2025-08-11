using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GestioneGiocatore : MonoBehaviour
{
    public GameObject personaggioGiocatore;
    public Button buttonTorneo;
    public Button buttonFacile;
    public Button buttonMedio;
    public Button buttonDifficile;

    private Button[] bottoni;
    private int indiceSelezionato = 0;

    private RacchettaManager racchettaManager;

    public AudioClip suonoSelezione;
    public AudioClip suonoConferma;
    private AudioSource audioSource;


    void Start()
    {
        racchettaManager = RacchettaManager.Instance;
        racchettaManager.ConnectionEstablished += OnConnectionEstablished;
        racchettaManager.DataReceived += DataReceived;

        // Recupera il giocatore selezionato
        string giocatoreSelezionato = PlayerPrefs.GetString("giocatore");
        personaggioGiocatore = CaricaPersonaggio.SostituisciConPrefab(personaggioGiocatore, giocatoreSelezionato);

        // Inizializza l'array dei bottoni
        bottoni = new Button[] { buttonTorneo, buttonFacile, buttonMedio, buttonDifficile };

        buttonTorneo.onClick.AddListener(() => { ScenaSuccessiva("torneo"); });
        buttonFacile.onClick.AddListener(() => { ScenaSuccessiva("AvvFacile"); });
        buttonMedio.onClick.AddListener(() => { ScenaSuccessiva("AvvMedio"); });
        buttonDifficile.onClick.AddListener(() => { ScenaSuccessiva("AvvDifficile"); });
        
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Gestisci l'input della tastiera
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            SelezionaSinistra();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            SelezionaDestra();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfermaSelezione();
        } else if (Input.GetKeyDown(KeyCode.M))
        {
            if (Musica.instance != null)
                Musica.instance.MutaAudio(!Musica.instance.GetComponent<AudioSource>().mute);
        }
    }

    private void SelezionaSinistra()
    {
        indiceSelezionato = (indiceSelezionato - 1 + bottoni.Length) % bottoni.Length;
        SelezionaBottone(indiceSelezionato);

        // Riproduci il suono di selezione
        if (audioSource != null && suonoSelezione != null)
            audioSource.PlayOneShot(suonoSelezione);
    }

    private void SelezionaDestra()
    {
        indiceSelezionato = (indiceSelezionato + 1) % bottoni.Length;
        SelezionaBottone(indiceSelezionato);

        // Riproduci il suono di selezione
        if (audioSource != null && suonoSelezione != null)
            audioSource.PlayOneShot(suonoSelezione);
    }

    private void ConfermaSelezione()
    {
        bottoni[indiceSelezionato].onClick.Invoke();

        // Riproduci il suono di conferma
        if (audioSource != null && suonoConferma != null)
            audioSource.PlayOneShot(suonoConferma);
    }

    private void SelezionaBottone(int indice)
    {
        // Simula l'effetto hover sul bottone selezionato
        EventSystem.current.SetSelectedGameObject(bottoni[indice].gameObject);
    }

    public void ScenaSuccessiva(string nomeScena)
    {
        if (nomeScena == "torneo")
        {
            PlayerPrefs.SetString("bot-attuale", "AvvFacile");
            PlayerPrefs.SetString("torneo-mode", "true");
        }
        else
        {
            PlayerPrefs.SetString("bot-attuale", nomeScena);
            PlayerPrefs.SetString("torneo-mode", "false");
        }

        PlayerPrefs.Save();
        CaricaScena.Instance.CaricaScenaConTransizione(transform, "TennisScene", 1f);
    }

    void DataReceived(string data)
    {
        if (data == "SX")
        {
            SelezionaSinistra();
        }
        else if (data == "DX")
        {
            SelezionaDestra();
        }
        else if (data == "OK")
        {
            ConfermaSelezione();
        }
        else if (data == "MUSIC")
        {
            if (Musica.instance != null)
                Musica.instance.MutaAudio(!Musica.instance.GetComponent<AudioSource>().mute);
        }
    }

    void OnConnectionEstablished()
    {
        racchettaManager.SendData("CONTROL");
    }

    void OnDestroy()
    {
        racchettaManager.ConnectionEstablished -= OnConnectionEstablished;
        racchettaManager.DataReceived -= DataReceived;
    }
}
