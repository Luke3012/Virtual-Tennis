using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Giocatore : GiocatoreBase
{
    public Transform mira;
    public Transform palla;

    bool staColpendo;
    bool colpibile;
    bool inBattuta;

    Vector3 posizioneMiraIniziale;
    Vector3 posizioneIniziale;

    [SerializeField] Transform servizioDestra;
    [SerializeField] Transform servizioSinistra;

    bool destraServito = true;

    GestionePunteggio gestionePunteggio;
    CameraController cameraController;

    public Transform limiteSinistroCampo;
    public Transform limiteDestroCampo;

    public GameObject personaggioGiocatore;

    public GameObject pannelloPausa;
    public Button buttonRiprendiPartita;
    public Button buttonEsciPartita;
    private bool isPannelloVersusAttivo;
    private bool isPannelloPunteggiAttivo;

    private RacchettaManager racchettaManager;

    public AudioClip suonoFruscio;
    public AudioClip suonoPallaColpita;
    public AudioClip suonoPausa;
    public AudioClip suonoRiprendi;
    private AudioSource audioSource;

    void Start()
    {
        racchettaManager = RacchettaManager.Instance;
        racchettaManager.ConnectionEstablished += OnConnectionEstablished;
        racchettaManager.DataReceived += DataReceived;
        racchettaManager.SendData("PAUSE");

        velocita = 16f; // Velocità default per il movimento del giocatore

        // Ottieni il riferimento al componente Animator
        animator = GetComponent<Animator>();

        // Salva la posizione iniziale della mira e del giocatore
        posizioneMiraIniziale = mira.position;
        posizioneIniziale = transform.position;

        // Ottieni il riferimento al componente GestoreColpi
        gestoreColpi = GetComponent<GestoreColpi>();
        colpoAttuale = gestoreColpi.rotazioneSuperiore;

        gestionePunteggio = FindAnyObjectByType<GestionePunteggio>();

        cameraController = FindObjectOfType<CameraController>();

        //Imposta il pannello per la pausa
        pannelloPausa.SetActive(false);
        buttonRiprendiPartita.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            pannelloPausa.SetActive(false);
            cameraController.pannelloVersus.SetActive(isPannelloVersusAttivo);
            gestionePunteggio.pannelloPunteggi.SetActive(isPannelloPunteggiAttivo);
            racchettaManager.SendData("PAUSE");

            //cerca di capire se è in battuta
            if (inBattuta)
                racchettaManager.SendData("BATTUTA");
            else
                racchettaManager.SendData("SWING");

            RiproduciSuonoPausa(true);
        });
        buttonEsciPartita.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            CaricaScena.Instance.CaricaScenaConTransizione(transform, "SceltaGiocatore", 0f);
            racchettaManager.SendData("EXIT");
        });

        //Carica il giocatore scelto
        personaggioGiocatore = CaricaPersonaggio.SostituisciConPrefab(personaggioGiocatore, PlayerPrefs.GetString("giocatore"));

        audioSource = GetComponent<AudioSource>();

        //Cambia la musica
        if (Musica.instance != null)
        {
            Musica.instance.CambiaTraccia("traccia-partita");

            // Aggiorna il Companion dei dati attuali
            if (Musica.instance.GetComponent<AudioSource>().mute)
                racchettaManager.SendData("SOUNDOFF");
            else
                racchettaManager.SendData("SOUNDON");
        }

        ResetGiocatore();
    }

    private void Update()
    {
        if (gestionePunteggio.finePartita) return;

        GestisciPausa();
        Muovi();

        if (inBattuta)
        {
            GestisciBattuta();
        }
        else
        {
            GestisciColpo();
        }

        GestisciMira();

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (Musica.instance != null)
            {
                Musica.instance.MutaAudio(!Musica.instance.GetComponent<AudioSource>().mute);
                if (Musica.instance.GetComponent<AudioSource>().mute)
                    racchettaManager.SendData("SOUNDOFF");
                else
                    racchettaManager.SendData("SOUNDON");
            }
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            if (cameraController.inPrimaPersona)
                racchettaManager.SendData("FPOFF");
            else
                racchettaManager.SendData("FPON");

            cameraController.InquadraPrimaPersona();
        }
    }

    public override void EseguiColpo(bool seBattuta)
    {
        if (seBattuta)
        {
            GestisciBattuta();
        }
        else
        {
            GestisciColpo();
        }
    }

    public override void ResetGiocatore()
    {
        if (gestionePunteggio.finePartita)
        {
            pannelloPausa.SetActive(false);
            if (gestionePunteggio.turnoGiocatore)
            {
                animator.Play("gioc-felice");
            }
            else
            {
                animator.Play("gioc-triste");
            }
            return;
        }

        if (gestionePunteggio.turnoGiocatore)
        {
            inBattuta = true;
            racchettaManager.SendData("BATTUTA");
            GetComponent<BoxCollider>().enabled = false;

            Vector3 posizioneBattutaPalla;
            if (destraServito)
            {
                transform.position = servizioSinistra.position;
                posizioneBattutaPalla = servizioSinistra.position + new Vector3(0.2f, 1, 0);
            }
            else
            {
                transform.position = servizioDestra.position;
                posizioneBattutaPalla = servizioDestra.position + new Vector3(0.2f, 1, 0);
            }

            destraServito = !destraServito;

            palla.GetComponent<Palla>().ImpostaPosizioneBattuta(posizioneBattutaPalla);
        }
        else
        {
            inBattuta = false;
            racchettaManager.SendData("SWING");

            transform.position = new Vector3((servizioSinistra.position.x + servizioDestra.position.x) / 2, servizioSinistra.position.y, servizioSinistra.position.z);
        }
    }

    private void GestisciPausa()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pannelloPausa.activeSelf)
            {
                buttonRiprendiPartita.onClick.Invoke();
            }
            else
            {
                PausaGioco();
            }
        }
    }

    public override void Muovi()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if (!staColpendo && !inBattuta)
        {
            Vector3 dir = new Vector3(h, 0, 0);
            mira.Translate(dir.normalized * velocita * 2 * Time.deltaTime);

            Vector3 posizioneFuturaPalla = CalcolaPosizioneFuturaPalla();
            Vector3 direzioneMovimento = new Vector3(posizioneFuturaPalla.x - transform.position.x, 0, 0);

            // Movimento fluido verso la posizione futura della palla
            Vector3 nuovaPosizione = transform.position + direzioneMovimento.normalized * velocita * Time.deltaTime;

            // Se la palla è abbastanza vicina, il giocatore si ferma
            if (Mathf.Abs(direzioneMovimento.x) < 0.5f)
            {
                nuovaPosizione = transform.position;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, nuovaPosizione, Time.deltaTime * velocita);
            }

            // Limita la posizione del giocatore ai bordi del campo
            BoxCollider limiteSinistroCollider = limiteSinistroCampo.GetComponent<BoxCollider>();
            BoxCollider limiteDestroCollider = limiteDestroCampo.GetComponent<BoxCollider>();

            float limiteSinistro = limiteSinistroCollider.bounds.max.x;
            float limiteDestro = limiteDestroCollider.bounds.min.x;

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, limiteSinistro, limiteDestro),
                transform.position.y,
                transform.position.z
            );
        }
    }

    private void GestisciBattuta()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            EseguiColpo(gestoreColpi.servizioSlice, true);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            EseguiColpo(gestoreColpi.servizioKick, true);
        }

        if (Input.GetKeyUp(KeyCode.R) || Input.GetKeyUp(KeyCode.T))
        {
            GestisciBattutaExc();
        }
    }

    private void GestisciBattutaExc()
    {
        if (cameraController.inPunteggio) return;

        palla.GetComponent<Rigidbody>().useGravity = true;
        staColpendo = false;
        GetComponent<BoxCollider>().enabled = true;

        Vector3 direzione = mira.position - transform.position;
        Vector3 altezza = new Vector3(0, colpoAttuale.forzaAltezza, 0);
        palla.GetComponent<Rigidbody>().velocity = direzione.normalized * colpoAttuale.forzaColpo + altezza;

        animator.Play("servizio");
        palla.GetComponent<Palla>().colpitaDa = "giocatore";
        palla.GetComponent<Palla>().inGioco = true;
        palla.GetComponent<Palla>().rimbalzi = 0;
        inBattuta = false;
        racchettaManager.SendData("SWING");

        RiproduciSuonoPalla(true);

        cameraController.InquadraGioco();
    }

    private void GestisciColpo()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EseguiColpo(gestoreColpi.rotazioneSuperiore, false);
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            staColpendo = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EseguiColpo(gestoreColpi.piatto, false);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            staColpendo = false;
        }
    }

    private void GestisciMira()
    {
        if (staColpendo && colpibile)
        {
            Vector3 direzionePalla = palla.position - transform.position;
            mira.position = new Vector3(
                transform.position.x + (direzionePalla.x >= 0 ? 1 : -1),
                mira.position.y,
                mira.position.z
            );

            // Limita la mira ai bordi del campo
            BoxCollider limiteSinistroCollider = limiteSinistroCampo.GetComponent<BoxCollider>();
            BoxCollider limiteDestroCollider = limiteDestroCampo.GetComponent<BoxCollider>();

            float limiteSinistro = limiteSinistroCollider.bounds.max.x + 1;
            float limiteDestro = limiteDestroCollider.bounds.min.x - 1;

            if (transform.position.x <= limiteSinistro)
            {
                mira.position = new Vector3(transform.position.x + 2, mira.position.y, mira.position.z);
            }
            else if (transform.position.x >= limiteDestro)
            {
                mira.position = new Vector3(transform.position.x - 2, mira.position.y, mira.position.z);
            }
            else
            {
                mira.position = new Vector3(
                    Mathf.Clamp(mira.position.x, limiteSinistro, limiteDestro),
                    mira.position.y,
                    mira.position.z
                );
            }

            Vector3 direzione = new Vector3(mira.position.x - transform.position.x, 0, mira.position.z - transform.position.z);
            palla.GetComponent<Rigidbody>().velocity = direzione.normalized * colpoAttuale.forzaColpo + new Vector3(0, colpoAttuale.forzaAltezza, 0);

            //colpibile = false;
        }
    }

    private void EseguiColpo(Colpo tipoColpo, bool seBattuta)
    {
        if (cameraController.inPunteggio) return;

        colpoAttuale = tipoColpo;
        ColpoPalla(seBattuta);
    }


    private Vector3 CalcolaPosizioneFuturaPalla()
    {
        // Calcola la posizione futura della palla basata sulla sua velocità e direzione solo sull'asse x
        Rigidbody rbPalla = palla.GetComponent<Rigidbody>();
        Vector3 velocitaPalla = rbPalla.velocity;

        // Calcola il tempo di volo stimato della palla
        float tempo = Mathf.Abs((palla.position.y - transform.position.y) / velocitaPalla.y);

        // Calcola la posizione futura della palla
        Vector3 posizioneFutura = palla.position + velocitaPalla * tempo;

        // Considera solo l'asse x per il movimento del giocatore
        posizioneFutura = new Vector3(posizioneFutura.x, transform.position.y, transform.position.z);

        return posizioneFutura;
    }


    private void ColpoPalla(bool seBattuta)
    {
        staColpendo = true;

        if (!seBattuta)
        {
            palla.GetComponent<Palla>().inGioco = true;
            palla.GetComponent<Palla>().colpitaDa = "giocatore";
            palla.GetComponent<Palla>().rimbalzi = 0;

            Vector3 direzionePalla = palla.position - transform.position;
            if (direzionePalla.x >= 0)
                animator.Play("dritto");
            else
                animator.Play("rovescio");

            if (colpibile)
                RiproduciSuonoPalla(true);
            else
                RiproduciSuonoPalla(false);
        }
        else
        {
            animator.Play("prepara-servizio");
        }

        // Resetta la posizione della mira
        mira.position = posizioneMiraIniziale;
    }

    private void RiproduciSuonoPalla(bool seColpita = false)
    {
        if (audioSource != null && suonoFruscio != null)
        {
            if (seColpita)
                audioSource.PlayOneShot(suonoPallaColpita);
            else
                audioSource.PlayOneShot(suonoFruscio);
        }
    }

    private void RiproduciSuonoPausa(bool seRiprendi = false)
    {
        if (audioSource != null && suonoPausa != null)
        {
            if (seRiprendi)
                audioSource.PlayOneShot(suonoRiprendi);
            else
                audioSource.PlayOneShot(suonoPausa);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Palla"))
        {
            //Debug.Log("La palla è colpibile!");
            colpibile = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Palla"))
        {
            //Debug.Log("La palla non è più colpibile!");
            colpibile = false;
        }
    }

    void PausaGioco()
    {
        Time.timeScale = 0;
        pannelloPausa.SetActive(true);

        isPannelloVersusAttivo = cameraController.pannelloVersus.activeSelf;
        isPannelloPunteggiAttivo = gestionePunteggio.pannelloPunteggi.activeSelf;
        cameraController.pannelloVersus.SetActive(false);
        gestionePunteggio.pannelloPunteggi.SetActive(false);
        racchettaManager.SendData("RESUME");

        RiproduciSuonoPausa(false);
    }

    void OnConnectionEstablished()
    {
        if (gestionePunteggio.finePartita) return;

        if (pannelloPausa.activeSelf)
        {
            racchettaManager.SendData("RESUME");
        }
        else
        {
            racchettaManager.SendData("PAUSE");
        }
    }
    void DataReceived(string data)
    {
        if (data == "RESUME")
        {
            buttonRiprendiPartita.onClick.Invoke();
        }
        else if (data == "PAUSE")
        {
            if (!pannelloPausa.activeSelf)
            {
                PausaGioco();
            }
        }
        else if (data == "EXIT")
        {
            buttonEsciPartita.onClick.Invoke();
        }
        else if (data == "PRIMAPERSONA")
        {
            cameraController.InquadraPrimaPersona();
        }
        else if (data == "MUSIC")
        {
            if (Musica.instance != null)
                Musica.instance.MutaAudio(!Musica.instance.GetComponent<AudioSource>().mute);
        }
        else if (data == "SWING_FORTE" && (palla.GetComponent<Palla>().inGioco || inBattuta))
        {
            if (!inBattuta)
            {
                EseguiColpo(gestoreColpi.rotazioneSuperiore, false);
                //staColpendo = false;
                StartCoroutine(ResetStaColpendo(0.3f));
            }
            else
            {
                EseguiColpo(gestoreColpi.servizioKick, true);
                //GestisciBattutaExc();
                StartCoroutine(ResetStaColpendo(0.3f, true));
            }
        }
        else if (data == "SWING_DEBOLE" && (palla.GetComponent<Palla>().inGioco || inBattuta))
        {
            if (!inBattuta)
            {
                EseguiColpo(gestoreColpi.piatto, false);
                //staColpendo = false;
                StartCoroutine(ResetStaColpendo(0.3f));
            }
            else
            {
                EseguiColpo(gestoreColpi.servizioSlice, true);
                //GestisciBattutaExc();
                StartCoroutine(ResetStaColpendo(0.3f, true));
            }
        }
    }

    IEnumerator ResetStaColpendo(float seconds = 1f, bool isBattuta = false)
    {
        yield return new WaitForSeconds(seconds);

        if (!isBattuta)
            staColpendo = false;
        else
            GestisciBattutaExc();
    }

    void OnDestroy()
    {
        racchettaManager.ConnectionEstablished -= OnConnectionEstablished;
        racchettaManager.DataReceived -= DataReceived;
    }
}

