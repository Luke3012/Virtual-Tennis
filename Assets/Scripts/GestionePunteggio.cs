using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GestionePunteggio : MonoBehaviour
{
    public TMP_Text punteggioGiocatoreText;
    public TMP_Text punteggioBotText;
    public TMP_Text punteggioText;
    public TMP_Text puntoText;
    public TMP_Text punteggio2Text;
    public TMP_Text punteggio3Text;
    public TMP_Text risultatoFineText;

    private Punteggio punteggioGiocatore = new Punteggio();
    private Punteggio punteggioBot = new Punteggio();

    public bool turnoGiocatore = true;
    public bool finePartita = false;

    private Arbitro arbitro;
    private Giocatore giocatore;
    private Bot bot;
    private CameraController cameraController;
    public GameObject pannelloPunteggi;

    public GameObject pannelloFinePartita;
    public GameObject pannelloProseguiTorneo;
    public GameObject pannelloEsci;
    public Button bottoneProseguiTorneo;
    public Button[] bottoneEsci;
    private CanvasGroup pannelloFinePartitaCanvasGroup;

    public Camera mainCamera; // La camera principale
    public Camera renderCamera; // La camera per il rendering

    private RacchettaManager racchettaManager;

    private IDifficolta difficolta;
    private Game game;


    void Start()
    {
        racchettaManager = RacchettaManager.Instance;
        racchettaManager.ConnectionEstablished += OnConnectionEstablished;
        racchettaManager.DataReceived += DataReceived;

        punteggio3Text.text = "";
        punteggioGiocatore.ResetPunti();
        punteggioBot.ResetPunti();
        aggiornaPunteggio();
        arbitro = FindObjectOfType<Arbitro>();
        giocatore = FindObjectOfType<Giocatore>();
        bot = FindObjectOfType<Bot>();
        cameraController = FindObjectOfType<CameraController>();

        finePartita = false;
        pannelloPunteggi.SetActive(false);
        punteggio2Text.gameObject.SetActive(false);
        pannelloFinePartita.SetActive(false);
        renderCamera.gameObject.SetActive(false);

        pannelloFinePartitaCanvasGroup = pannelloFinePartita.GetComponentInParent<CanvasGroup>();
        pannelloFinePartitaCanvasGroup.alpha = 0f;

        bottoneProseguiTorneo.onClick.AddListener(() =>
        {
            ExitScene(false);
        });

        foreach (Button bottone in bottoneEsci)
        {
            bottone.onClick.AddListener(() =>
            {
                ExitScene(true);
            });
        }

        // Decidi la difficoltà in base alle preferenze del giocatore
        string difficoltaAttuale = PlayerPrefs.GetString("bot-attuale");
        GestoreColpi gestoreColpi = FindObjectOfType<GestoreColpi>();

        if (difficoltaAttuale == "AvvFacile")
        {
            difficolta = new DifficoltaFacile(gestoreColpi);
            game = new Game(1);
        }
        else if (difficoltaAttuale == "AvvMedio")
        {
            difficolta = new DifficoltaMedia(gestoreColpi);
            game = new Game(2);
        }
        else if (difficoltaAttuale == "AvvDifficile")
        {
            difficolta = new DifficoltaDifficile(gestoreColpi);
            game = new Game(3);
        }

        bot.ImpostaDifficolta(difficolta);

        // Avvia la coroutine per inquadrare l'avversario e poi la battuta
        StartCoroutine(InquadraAvversarioPrimaDiIniziare());

        /*Test per la vittoria
        gameGiocatore = 1;
        gameBot= 0;
        finePartita = true;
        PlayerPrefs.SetString("torneo-mode", "false");
        PlayerPrefs.SetString("bot-attuale", "AvvFacile");
        ControllaVittoria();
        CambiaTurno();
        */
    }

    IEnumerator InquadraAvversarioPrimaDiIniziare()
    {
        cameraController.InquadraVersus();
        yield return new WaitForSeconds(4); // Aspetta qualche secondo
        cameraController.InquadraBattuta();
    }

    public void AggiungiPunto(string colpitaDa, string motivo = "")
    {
        bool giocatoreHaVinto = false;

        if (colpitaDa == "giocatore" || colpitaDa == "botfuoricampo")
        {
            punteggioGiocatore.AggiungiPunto();
            puntoText.text = "Punto a <b>giocatore</b>!";
            giocatoreHaVinto = true;
            bot.GetComponent<Animator>().Play("bot-triste");
        }
        else if (colpitaDa == "bot" || colpitaDa == "giocatorefuoricampo")
        {
            punteggioBot.AggiungiPunto();
            puntoText.text = "Punto a <b>bot</b>!";
            bot.GetComponent<Animator>().Play("bot-felice");
        }

        punteggio3Text.text = motivo;
        aggiornaPunteggio();
        ControllaVittoria();
        StartCoroutine(PausaPunteggio(giocatoreHaVinto));
    }

    void aggiornaPunteggio()
    {
        punteggioText.text = punteggioGiocatore.PunteggioToString() + " - " + punteggioBot.PunteggioToString();
        punteggio2Text.text = punteggioText.text;
        punteggio2Text.gameObject.SetActive(false);
        punteggio3Text.gameObject.SetActive(false);
        pannelloPunteggi.SetActive(true);
    }

    void ControllaVittoria()
    {
        if (punteggioGiocatore.GetPunti() == 50)
        {
            game.AggiungiGameGiocatore();
            punteggioGiocatoreText.text = "Giocatore " + game.GameToString(game.GetGameGiocatore());
            punteggioGiocatore.ResetPunti();
            punteggioBot.ResetPunti();
            Debug.Log("Il giocatore ha vinto il game!");
            puntoText.text = "Gioco di <b>giocatore</b>!";
            punteggio2Text.text = game.GameToString(game.GetGameGiocatore()) + " - " + game.GameToString(game.GetGameBot());
        }
        else if (punteggioBot.GetPunti() == 50)
        {
            game.AggiungiGameBot();
            punteggioBotText.text = "Bot " + game.GameToString(game.GetGameBot());
            punteggioGiocatore.ResetPunti();
            punteggioBot.ResetPunti();
            Debug.Log("Il bot ha vinto il game!");
            puntoText.text = "Gioco di <b>bot</b>!";
            punteggio2Text.text = game.GameToString(game.GetGameGiocatore()) + " - " + game.GameToString(game.GetGameBot());
        }

        if (game.HaVintoGiocatore())
        {
            Debug.Log("Il giocatore ha vinto la partita!");
            puntoText.text = "<b>Giocatore</b> ha vinto la partita!";
            punteggio2Text.text = "Congratulazioni";
            finePartita = true;
        }
        else if (game.HaVintoBot())
        {
            Debug.Log("Il bot ha vinto la partita!");
            puntoText.text = "<b>Bot</b> ha vinto la partita!";
            punteggio2Text.text = "Ritenta...";
            finePartita = true;
        }
    }



    IEnumerator PausaPunteggio(bool giocatoreHaVinto)
    {
        cameraController.InquadraPunteggio();
        arbitro.ReagisciAlPunto(giocatoreHaVinto);

        yield return new WaitForSeconds(5); // Aspetta 5 secondi

        arbitro.FineReazione();
        CambiaTurno();
    }

    IEnumerator FadePannelloFinePartita()
    {
        float duration = 1f; // Durata della transizione in secondi
        float elapsedTime = 0f;
        float startAlpha = pannelloFinePartitaCanvasGroup.alpha;
        float targetAlpha = 1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            pannelloFinePartitaCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        pannelloFinePartitaCanvasGroup.alpha = targetAlpha;
    }

    void CambiaTurno()
    {
        turnoGiocatore = !turnoGiocatore;
        if (finePartita)
        {
            GameObject daMostrare = null;

            renderCamera.gameObject.SetActive(true);
            mainCamera.gameObject.SetActive(false);
            pannelloEsci.SetActive(true);
            pannelloProseguiTorneo.SetActive(false);

            if (game.HaVintoGiocatore())
            {
                turnoGiocatore = true; // Indica alle altre classi che ha vinto il giocatore
                risultatoFineText.text = "Vittoria!";

                if (PlayerPrefs.GetString("torneo-mode") == "true")
                {
                    if (PlayerPrefs.GetString("bot-attuale") == "AvvFacile")
                    {
                        daMostrare = Instantiate(Resources.Load<GameObject>("CoppaBronzo"));
                        racchettaManager.SendData("ANIMBRONZO");
                        PlayerPrefs.SetString("bot-attuale", "AvvMedio");
                        pannelloProseguiTorneo.SetActive(true);
                        pannelloEsci.SetActive(false);
                    }
                    else if (PlayerPrefs.GetString("bot-attuale") == "AvvMedio")
                    {
                        daMostrare = Instantiate(Resources.Load<GameObject>("CoppaArgento"));
                        racchettaManager.SendData("ANIMARGENTO");
                        PlayerPrefs.SetString("bot-attuale", "AvvDifficile");
                        pannelloEsci.SetActive(false);
                        pannelloProseguiTorneo.SetActive(true);
                    }
                    else
                    {
                        daMostrare = Instantiate(Resources.Load<GameObject>("CoppaOro"));
                        racchettaManager.SendData("ANIMORO");
                    }
                }
                else
                {
                    //daMostrare = Instantiate(Resources.Load<GameObject>("Arbitro"));
                    racchettaManager.SendData("ANIMVITTORIA");
                }
            }
            else if (game.HaVintoBot())
            {
                turnoGiocatore = false; // Indica alle altre classi che ha vinto il bot
                risultatoFineText.text = "Sconfitta.";

                //daMostrare = Instantiate(giocatore.gameObject);
                racchettaManager.SendData("ANIMSCONFITTA");
            }

            //Adesso mostra l'oggetto della vittoria
            if (daMostrare != null)
            {
                daMostrare.transform.position = renderCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2 - 80f, 10f));
                daMostrare.transform.SetParent(pannelloFinePartita.transform);
                ImpostaLayerUI(daMostrare);
                CreaLuceDirezionale(daMostrare);

                daMostrare.transform.rotation = Quaternion.identity; // Resetta la rotazione
                StartCoroutine(RuotaOggetto(daMostrare)); // Ruota l'oggetto attorno a se stesso
            }

            pannelloFinePartita.SetActive(true);
            StartCoroutine(FadePannelloFinePartita());
        }

        GameObject.Find("giocatore").GetComponent<Giocatore>().ResetGiocatore();
        GameObject.Find("bot").GetComponent<Bot>().ResetGiocatore();
        pannelloPunteggi.SetActive(false);
    }

    void ImpostaLayerUI(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("UI");
        foreach (Transform child in obj.transform)
        {
            ImpostaLayerUI(child.gameObject);
        }
    }

    void CreaLuceDirezionale(GameObject target)
    {
        GameObject luceObj = new GameObject("LuceDirezionale");
        Light luce = luceObj.AddComponent<Light>();
        luce.type = LightType.Point;
        luce.color = Color.white;
        luce.intensity = 5.0f;

        // Posiziona la luce in modo che illumini l'oggetto
        luceObj.transform.position = target.transform.position + new Vector3(5, 1, -7);
        luceObj.transform.LookAt(target.transform);
    }

    IEnumerator RuotaOggetto(GameObject daMostrare)
    {
        while (true)
        {
            daMostrare.transform.Rotate(new Vector3(0, 1, 0), 1f); // Ruota in modo obliquo
            yield return null;
        }
    }

    void ExitScene(bool forceExit = true)
    {
        // Se è in corso un torneo e il giocatore ha vinto, passa al prossimo avversario
        if (PlayerPrefs.GetString("torneo-mode") == "true" && turnoGiocatore && !forceExit)
        {
            racchettaManager.SendData("PAUSE");
            CaricaScena.Instance.CaricaScenaConTransizione(transform, "TennisScene", 0f);
        } else
        {
            racchettaManager.SendData("EXIT");
            CaricaScena.Instance.CaricaScenaConTransizione(transform, "SceltaGiocatore", 0f);
        }       
    }

    void OnConnectionEstablished()
    {
        if (finePartita) {
            racchettaManager.SendData("CALIBRATE");
        }
    }
    void DataReceived(string data)
    {
        if (data == "CONFIRM")
        {
            ExitScene(false);
        }
    }
    void OnDestroy()
    {
        racchettaManager.ConnectionEstablished -= OnConnectionEstablished;
        racchettaManager.DataReceived -= DataReceived;
    }
}
