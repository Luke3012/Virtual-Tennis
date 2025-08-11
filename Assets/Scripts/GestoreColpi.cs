using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Permette di visualizzare la classe in Unity
public class Colpo
{
    public float forzaAltezza;
    public float forzaColpo;
}

public class GestoreColpi : MonoBehaviour
{
    public Colpo rotazioneSuperiore;
    public Colpo piatto;
    public Colpo servizioSlice;
    public Colpo servizioKick;
}
