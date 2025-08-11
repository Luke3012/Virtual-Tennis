using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using I2.TextAnimation;
using System.Data;

public class Menu : MonoBehaviour
{
    public TMP_Text bigText;
    public TMP_Text ipText;

    private RacchettaManager racchettaManager;

    void Start()
    {
        racchettaManager = RacchettaManager.Instance;
        racchettaManager.ConnectionEstablished += OnConnectionEstablished;
        racchettaManager.DataReceived += DataReceived;


        ipText.text = "<b>Connettiti qui</b>\n";

        // Ottieni l'indirizzo IP del PC
        string ipAddress = GetIPAddress();
        ipText.text += ipAddress;
    }

    void Update()
    {
        //Qui ci dovrà essere il collegamento con lo smartphone, ma per il momento rimaniamo le cose così
        if (Input.GetKeyDown(KeyCode.Return))
        {    
            StartCoroutine(LoadSceneWithDelay(0f, "SceltaGiocatore"));
        } else if (Input.GetKeyDown(KeyCode.M))
        {
            if (Musica.instance != null)
                Musica.instance.MutaAudio(!Musica.instance.GetComponent<AudioSource>().mute);
        }
    }

    string GetIPAddress()
    {
        string ipAddress = "";
        System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (System.Net.IPAddress address in hostEntry.AddressList)
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ipAddress = address.ToString();
                break;
            }
        }
        return ipAddress;
    }

    IEnumerator LoadSceneWithDelay(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        racchettaManager.SendData("CONTROL");
        ipText.GetComponent<TextAnimation>().StopAllAnimations();
        ipText.GetComponent<TextAnimation>().PlayAnim(1);
        bigText.GetComponent<TextAnimation>().PlayAnim(1);
        CaricaScena.Instance.CaricaScenaConTransizione(transform, sceneName);
    }

    void OnConnectionEstablished()
    {
        ipText.text = "E' ora di calibrare! Mettiti in posizione d'attesa e premi <b>CONFERMA</b>";
        racchettaManager.SendData("CALIBRATE");
    }
    void DataReceived(string data)
    {
        if (data == "CALIBRATED")
        {
            StartCoroutine(LoadSceneWithDelay(0f, "SceltaGiocatore"));
        } else if (data == "ERRORE")
        {
            ipText.text = "<b>Companion</b>Errore di calibrazione!";
        } else if (data == "MUSIC")
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
