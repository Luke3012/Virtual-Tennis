using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class RacchettaManager : MonoBehaviour
{
    private static RacchettaManager _instance;
    public event Action ConnectionEstablished;
    public event Action ConnectionLost;
    public event Action<string> DataReceived;

    // Coda per azioni da eseguire nel thread principale
    private Queue<Action> mainThreadActions = new Queue<Action>();

    public static RacchettaManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RacchettaManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(RacchettaManager).ToString());
                    _instance = singleton.AddComponent<RacchettaManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    public GameObject racchetta;
    public GameObject palla;

    bool isConnected = false;
    private TcpListener server;
    private TcpClient client;
    private Thread serverThread;
    private UdpClient udpListener;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "MenuPrincipale")
        {
            racchetta = GameObject.FindGameObjectWithTag("Racchetta");
            palla = GameObject.FindGameObjectWithTag("Palla");
        }

        StartServer();
        StartUdpListener(); // Aggiungiamo il listener UDP per la scoperta
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MenuPrincipale")
        {
            if (!isConnected)
            {
                racchetta.GetComponentInParent<Animator>().Play("racchettastandby");
            }
            else
            {
                //racchetta.GetComponentInParent<Animator>().StopPlayback();
                ApplyRotation();
            }
        }

        // Processa azioni in coda dal thread principale
        lock (mainThreadActions)
        {
            while (mainThreadActions.Count > 0)
            {
                mainThreadActions.Dequeue().Invoke();
            }
        }
    }

    void ApplyRotation()
    {
        // Applicare la rotazione alla racchetta con interpolazione per rendere il movimento fluido  
        Vector3 targetPosition = new Vector3(318f, 160f, -415f);
        Quaternion targetRotation = Quaternion.Euler(76.82576f, 295.1203f, 265.1979f);
        racchetta.GetComponentInParent<Transform>().SetPositionAndRotation(
            Vector3.Slerp(racchetta.GetComponentInParent<Transform>().position, targetPosition, Time.deltaTime * 2f),
            Quaternion.Slerp(racchetta.GetComponentInParent<Transform>().rotation, targetRotation, Time.deltaTime * 2f)
        );
    }

    void StartServer()
    {
        serverThread = new Thread(new ThreadStart(ServerThread));
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    void ServerThread()
    {
        server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Debug.Log("Server avviato sulla porta 5000");

        while (true)
        {
            client = server.AcceptTcpClient();
            isConnected = true;
            Debug.Log("Connesso al client");
            mainThreadActions.Enqueue(() =>
            {
                // Notifica il menu che la connessione è stata stabilita
                ConnectionEstablished?.Invoke();
            });

            // Handle client in a separate thread
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
            clientThread.IsBackground = true;
            clientThread.Start(client);
        }
    }

    void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Debug.Log("Dati ricevuti: " + data);

            if (data.Contains("PING"))
            {
                // Invia il messaggio "PONG" al client
                SendData("PONG");
                data = data.Replace("PING", "");
            }
            mainThreadActions.Enqueue(() =>
            {
                DataReceived?.Invoke(data); //SX - OK - DX - RESUME - EXIT - PAUSE - GESTURE?
            });
        }

        client.Close();
        isConnected = false;
        Debug.Log("Client disconnesso");
    }

    public void SendData(string message)
    {
        if (client != null && client.Connected)
        {
            NetworkStream stream = client.GetStream();
            if (stream.CanWrite)
            {
                try
                {
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Debug.Log("Invio al Companion: " + message);
                }
                catch (IOException ex)
                {
                    Debug.LogError("Errore nell'invio dei dati: " + ex.Message);
                }
            }
        }
    }

    void StartUdpListener()
    {
        udpListener = new UdpClient(5001);  // Listen on port 5001 for discovery messages
        udpListener.EnableBroadcast = true;

        Thread udpThread = new Thread(new ThreadStart(UdpListenerThread));
        udpThread.IsBackground = true;
        udpThread.Start();
    }

    void UdpListenerThread()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 5001);
        while (true)
        {
            byte[] data = udpListener.Receive(ref endPoint);
            string message = Encoding.ASCII.GetString(data);
            if (message == "DISCOVER_SERVER")
            {
                Debug.Log("Ricevuto messaggio di scoperta dal Companion");
                byte[] response = Encoding.ASCII.GetBytes("SERVER_RESPONSE");
                udpListener.Send(response, response.Length, endPoint);
                Debug.Log("Risposta del server inviata a: " + endPoint.Address.ToString());
            }
        }
    }

    void OnApplicationQuit()
    {
        if (server != null)
        {
            server.Stop();
        }

        if (serverThread != null)
        {
            serverThread.Abort();
        }

        if (udpListener != null)
        {
            udpListener.Close();
        }
    }
}
