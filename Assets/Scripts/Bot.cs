using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Bot : GiocatoreBase
{
    public Transform mira;
    public Transform palla;

    float timerBattuta = 0f;
    bool prontoABattere = false;
    float accuratezza = 1f;
    float probabilitaMiraFuori = 0f;
    float probabilitaErrore = 0f;

    public Transform[] mire;
    Vector3 posizioneMira;
    Vector3 posizioneBattuta;
    GestionePunteggio gestionePunteggio;
    CameraController cameraController;
    public Transform limiteSinistroCampo;
    public Transform limiteDestroCampo;
    public GameObject personaggioGiocatore;
    public AudioClip suonoFruscio;
    public AudioClip suonoPallaColpita;
    private AudioSource audioSource;

    private IDifficolta difficolta;

    void Start()
    {
        animator = GetComponent<Animator>();
        posizioneMira = transform.position;
        posizioneBattuta = transform.position;
        gestoreColpi = GetComponent<GestoreColpi>();
        gestionePunteggio = FindObjectOfType<GestionePunteggio>();
        cameraController = FindObjectOfType<CameraController>();

        // Carica il personaggio del bot scelto
        personaggioGiocatore = CaricaPersonaggio.SostituisciConPrefab(personaggioGiocatore, PlayerPrefs.GetString("bot-attuale"));

        audioSource = GetComponent<AudioSource>();

        ResetGiocatore();
    }

    public void ImpostaDifficolta(IDifficolta nuovaDifficolta)
    {
        difficolta = nuovaDifficolta;
        velocita = difficolta.CalcolaVelocita();
        accuratezza = difficolta.Accuratezza();
        probabilitaMiraFuori = difficolta.ProbabilitaMiraFuori();
        probabilitaErrore = difficolta.ProbabilitaErrore();
    }


    private void Update()
    {
        if (gestionePunteggio.finePartita || cameraController.inPunteggio) return;
        Muovi();

        if (prontoABattere)
        {
            timerBattuta += Time.deltaTime;
            if (timerBattuta >= 1f) // Se il timer raggiunge 1 secondo, il bot batte
            {
                EseguiColpo(true);
                timerBattuta = 0f;
                prontoABattere = false;
            }
        }
    }

    public override void Muovi()
    {
        posizioneMira.x = palla.position.x * accuratezza; // Il bot segue la palla con meno precisione

        Vector3 nuovaPosizione = Vector3.MoveTowards(transform.position, posizioneMira, velocita * Time.deltaTime);

        // Limita la posizione del bot ai bordi del campo
        BoxCollider limiteSinistroCollider = limiteSinistroCampo.GetComponent<BoxCollider>();
        BoxCollider limiteDestroCollider = limiteDestroCampo.GetComponent<BoxCollider>();

        float limiteSinistro = limiteSinistroCollider.bounds.max.x;
        float limiteDestro = limiteDestroCollider.bounds.min.x;

        nuovaPosizione.x = Mathf.Clamp(nuovaPosizione.x, limiteSinistro, limiteDestro);

        transform.position = nuovaPosizione;
    }

    public override void EseguiColpo(bool seBattuta)
    {
        Colpo colpoAttuale = ScegliColpo(seBattuta);
        animator.Play(seBattuta ? "prepara-servizio" : "rovescio");

        palla.GetComponent<Palla>().colpitaDa = "bot";
        palla.GetComponent<Palla>().inGioco = true;

        palla.GetComponent<Rigidbody>().useGravity = true;

        Vector3 direzione = mira.position - transform.position; // Calcola la direzione del colpo in base alla posizione della mira
        Vector3 altezza = new Vector3(0, colpoAttuale.forzaAltezza, 0); // Aggiungi una forza verticale al colpo
        palla.GetComponent<Rigidbody>().velocity = direzione.normalized * colpoAttuale.forzaColpo + altezza; // Applica la forza alla palla
        animator.Play(seBattuta ? "servizio" : "rovescio"); // Esegui l'animazione del colpo
        palla.GetComponent<Palla>().colpitaDa = "bot";

        RiproduciSuono(true);

        cameraController.InquadraGioco();
    }

    public override void ResetGiocatore()
    {
        transform.position = posizioneBattuta;

        if (gestionePunteggio.turnoGiocatore)
        {
            timerBattuta = 0f; // Resetta il timer se non è il turno del bot
            prontoABattere = false;
        }
        else
        {
            prontoABattere = true;
            Vector3 posizioneBattutaPalla = transform.position + new Vector3(0.2f, 1, 0); // Posizione della palla per la battuta
            palla.GetComponent<Palla>().ImpostaPosizioneBattuta(posizioneBattutaPalla);
        }
    }

    private Vector3 ScegliMira()
    {
        int randomico = Random.Range(0, mire.Length);

        // Se il bot mira fuori, scegli una posizione fuori dai limiti del campo
        if (Random.value < probabilitaMiraFuori)
        {
            return new Vector3(limiteDestroCampo.position.x + 1f, mire[randomico].position.y, mire[randomico].position.z);
        }

        return mire[randomico].position;
    }

    private Colpo ScegliColpo(bool seBattuta)
    {
        int randomico = Random.Range(0, 2);

        if (seBattuta)
            return randomico == 0 ? gestoreColpi.servizioSlice : gestoreColpi.servizioKick;
        else
            return randomico == 0 ? gestoreColpi.rotazioneSuperiore : gestoreColpi.piatto;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gestionePunteggio.finePartita)
        {
            if (gestionePunteggio.turnoGiocatore)
            {
                animator.Play("bot-felice");
            }
            else
            {
                animator.Play("bot-triste");
            }
        }

        if (other.CompareTag("Palla"))
        {
            if (prontoABattere)
                return;

            // Bisogna comunque impostarlo altrimenti il punteggio sarà errato
            palla.GetComponent<Palla>().colpitaDa = "bot";
            palla.GetComponent<Palla>().rimbalzi = 0;

            // Genera un numero casuale per decidere se il bot fallirà
            if (Random.value < probabilitaErrore)
            {
                // Il bot fallisce nel colpire la palla
                Debug.Log("Il bot ha mancato la palla!");
                RiproduciSuono(false);
                return;
            }

            // Se non ha fallito, esegui il colpo normalmente
            Colpo colpoAttuale = ScegliColpo(false);
            Vector3 direzione = ScegliMira() - transform.position;
            Vector3 altezza = new Vector3(0, colpoAttuale.forzaAltezza, 0);
            other.GetComponent<Rigidbody>().velocity = direzione.normalized * colpoAttuale.forzaColpo + altezza;

            Vector3 direzionePalla = palla.position - transform.position;
            if (direzionePalla.x >= 0)
                animator.Play("dritto");
            else
                animator.Play("rovescio");

            RiproduciSuono(true);
        }
    }

    private void RiproduciSuono(bool seColpita = false)
    {
        if (audioSource != null && suonoFruscio != null)
        {
            if (seColpita)
                audioSource.PlayOneShot(suonoPallaColpita);
            else
                audioSource.PlayOneShot(suonoFruscio);
        }
    }
}
