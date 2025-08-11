using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using I2.TextAnimation;

public class SceltaGiocatore : MonoBehaviour
{
    [SerializeField] public GameObject racchettaLuca;
    [SerializeField] public GameObject racchettaLucrezia;
    public TMP_Text lucaText;
    public TMP_Text lucreziaText;
    public TMP_Text bigText;

    private bool isLucaSelected = true;

    private RacchettaManager racchettaManager;

    public AudioClip suonoSelezione;
    public AudioClip suonoConferma;
    private AudioSource audioSource;


    void Start()
    {
        racchettaManager = RacchettaManager.Instance;
        racchettaManager.ConnectionEstablished += OnConnectionEstablished;
        racchettaManager.DataReceived += DataReceived;

        racchettaLuca.SetActive(false);
        racchettaLucrezia.SetActive(false);

        if (Musica.instance != null)
            Musica.instance.CambiaTraccia("traccia-menu");
        
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            SelezionaLuca();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            SelezionaLucrezia();
        }else if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfermaSelezione();
        } else if (Input.GetKeyDown(KeyCode.M))
        {
            if (Musica.instance != null)
                Musica.instance.MutaAudio(!Musica.instance.GetComponent<AudioSource>().mute);
        }
    }

    void SelezionaLuca()
    {
        bigText.text = "Scegli il giocatore";
        lucaText.color = Color.yellow;
        lucreziaText.color = Color.white;
        isLucaSelected = true;
        racchettaLuca.SetActive(false);
        racchettaLucrezia.SetActive(false);

        // Riproduci il suono di selezione
        if (audioSource != null && suonoSelezione != null)
        {
            audioSource.PlayOneShot(suonoSelezione);
        }
    }

    void SelezionaLucrezia()
    {
        bigText.text = "Scegli il giocatore";
        lucaText.color = Color.white;
        lucreziaText.color = Color.yellow;
        isLucaSelected = false;
        racchettaLuca.SetActive(false);
        racchettaLucrezia.SetActive(false);

        // Riproduci il suono di selezione
        if (audioSource != null && suonoSelezione != null)
            audioSource.PlayOneShot(suonoSelezione);
    }

    void ConfermaSelezione()
    {
        if (bigText.text != "Confermi?")
        {
            bigText.text = "Confermi?";
            racchettaLuca.SetActive(isLucaSelected);
            racchettaLucrezia.SetActive(!isLucaSelected);
        }
        else
        {
            // Salva il giocatore selezionato
            PlayerPrefs.SetString("giocatore", isLucaSelected ? "Luca" : "Lucrezia");
            PlayerPrefs.Save();
            racchettaManager.SendData(isLucaSelected ? "LUCA" : "LUCREZIA");

            lucaText.GetComponent<TextAnimation>().PlayAnim(1);
            lucreziaText.GetComponent<TextAnimation>().PlayAnim(1);
            bigText.GetComponent<TextAnimation>().PlayAnim(1);
            CaricaScena.Instance.CaricaScenaConTransizione(transform, "SceltaAvversario", 1f);
        }

        // Riproduci il suono di conferma
        if (audioSource != null && suonoConferma != null)
            audioSource.PlayOneShot(suonoConferma);
    }

    void OnConnectionEstablished()
    {
        racchettaManager.SendData("CONTROL");
    }

    void DataReceived(string data)
    {
        if (data == "SX")
        {
            SelezionaLuca();
        }
        else if (data == "DX")
        {
            SelezionaLucrezia();
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

    void OnDestroy()
    {
        racchettaManager.ConnectionEstablished -= OnConnectionEstablished;
        racchettaManager.DataReceived -= DataReceived;
    }
}
