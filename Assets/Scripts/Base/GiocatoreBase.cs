using UnityEngine;

public abstract class GiocatoreBase : MonoBehaviour
{
    protected GestoreColpi gestoreColpi;
    protected Animator animator;
    protected Colpo colpoAttuale;
    protected float velocita;

    // Metodi per il movimento e la gestione dei colpi
    public abstract void Muovi();
    public abstract void EseguiColpo(bool seBattuta);
    public abstract void ResetGiocatore();
}
