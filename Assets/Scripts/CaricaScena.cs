using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaricaScena : MonoBehaviour
{
    public static CaricaScena Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CaricaScenaConTransizione(Transform cameraTransform, string nomeScena, float durata = 3f)
    {
        StartCoroutine(CaricaScenaConTransizioneCoroutine(cameraTransform, nomeScena, durata));
    }

    private IEnumerator CaricaScenaConTransizioneCoroutine(Transform cameraTransform, string nomeScena, float durata)
    {
        // Sposta la telecamera in avanti per 3 secondi
        float tempoTrascorso = 0f;
        Vector3 posizioneIniziale = cameraTransform.position;
        Vector3 posizioneObiettivo = posizioneIniziale + cameraTransform.forward * 1000f;

        while (tempoTrascorso < durata)
        {
            tempoTrascorso += Time.deltaTime;
            float t = tempoTrascorso / durata;
            cameraTransform.position = Vector3.Lerp(posizioneIniziale, posizioneObiettivo, t);
            yield return null;
        }

        // Carica la scena successiva in modo asincrono
        AsyncOperation caricamentoAsincrono = SceneManager.LoadSceneAsync(nomeScena);
        while (!caricamentoAsincrono.isDone)
        {
            yield return null;
        }

        // Scarica la scena corrente
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
