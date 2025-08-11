using UnityEngine;

public static class CaricaPersonaggio
{
    /// <summary>
    /// Sostituisce un oggetto nella scena con un prefab mantenendo la gerarchia, posizione e rotazione.
    /// </summary>
    /// <param name="originale">L'oggetto originale da sostituire.</param>
    /// <param name="nomeGiocatore">Il nome del player dal quale ne verrà ricavato il percorso.</param>
    /// <returns>Il nuovo oggetto istanziato.</returns>
    public static GameObject SostituisciConPrefab(GameObject originale, string nomeGiocatore)
    {
        if (originale == null)
        {
            Debug.LogError("L'oggetto originale non può essere nullo.");
            return null;
        }
        if (nomeGiocatore == null)
        {
            nomeGiocatore = "Luca";
            Debug.Assert(false, "Il nome del giocatore non può essere nullo. Impostato a Luca.");
        }

        string prefabPath = recuperaPercorsoPrefab(nomeGiocatore);

        // Carica il prefab dagli asset
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Prefab non trovato al percorso: " + prefabPath);
            return null;
        }

        // Salva il padre, la posizione e la rotazione locali dell'oggetto originale
        Transform padreOriginale = originale.transform.parent;
        Vector3 posizioneLocaleOriginale = originale.transform.localPosition;
        Quaternion rotazioneLocaleOriginale = originale.transform.localRotation;

        // Distruggi l'oggetto originale
        Object.Destroy(originale);

        // Istanzia il prefab come figlio dello stesso padre e nella stessa posizione e rotazione locali
        GameObject nuovoOggetto = Object.Instantiate(prefab, padreOriginale);
        nuovoOggetto.transform.localPosition = posizioneLocaleOriginale;
        nuovoOggetto.transform.localRotation = rotazioneLocaleOriginale;

        return nuovoOggetto;
    }

    static string recuperaPercorsoPrefab(string giocatoreSelezionato)
    {
        string prefabPath = null;
        if (giocatoreSelezionato == "Luca")
        {
            prefabPath = "PersonaggioM";
        }
        else if (giocatoreSelezionato == "Lucrezia")
        {
            prefabPath = "PersonaggioF";
        } else if (giocatoreSelezionato == "AvvFacile" || giocatoreSelezionato == "AvvMedio" || giocatoreSelezionato == "AvvDifficile")
        {
            prefabPath = giocatoreSelezionato;
        }
        else
        {
            Debug.LogError("Giocatore non riconosciuto: " + giocatoreSelezionato);
        }
        return prefabPath;
    }
}
