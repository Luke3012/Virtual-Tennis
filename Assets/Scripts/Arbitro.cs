using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arbitro : MonoBehaviour
{
    public Transform palla;
    private Vector3 posizioneIniziale;
    private Animator animator;

    public bool inReazione = false;

    void Start()
    {
        posizioneIniziale = transform.position;
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    void Update()
    {
        Vector3 direzione;
        if (inReazione)
        {
            // Ruota verso la posizione iniziale
            direzione = posizioneIniziale - transform.position;
        }
        else
        {
            // Ruota verso la palla
            direzione = palla.position - transform.position;
        }

        direzione.y = 0; // Mantieni la rotazione solo sull'asse Y
        if (direzione != Vector3.zero)
        {
            Quaternion rotazione = Quaternion.LookRotation(direzione);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotazione, Time.deltaTime * 2);
        }
    }

    public void ReagisciAlPunto(bool giocatoreHaVinto)
    {
        inReazione = true;
        animator.enabled = true;

        if (giocatoreHaVinto)
        {
            animator.Play("arbitro-felice");
        }
        else
        {
            animator.Play("arbitro-triste");
        }
    }

    public void FineReazione()
    {
        inReazione = false;
        animator.enabled = false;
        transform.position = posizioneIniziale;
    }
}

