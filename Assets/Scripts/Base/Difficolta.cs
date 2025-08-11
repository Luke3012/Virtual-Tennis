using UnityEngine;

public class DifficoltaFacile : IDifficolta
{
    private GestoreColpi gestoreColpi;

    public DifficoltaFacile(GestoreColpi gestoreColpi)
    {
        this.gestoreColpi = gestoreColpi;
    }

    public float CalcolaVelocita() => 30f; // Velocità default per il movimento del bot
    public float ProbabilitaErrore() => 0.3f;
    public float ProbabilitaMiraFuori() => 0.3f;
    public float Accuratezza() => 1f;
}

public class DifficoltaMedia : IDifficolta
{
    private GestoreColpi gestoreColpi;

    public DifficoltaMedia(GestoreColpi gestoreColpi)
    {
        this.gestoreColpi = gestoreColpi;
    }

    public float CalcolaVelocita() => 35f; // 30f + 5f
    public float ProbabilitaErrore() => 0.2f;
    public float ProbabilitaMiraFuori() => 0.2f;
    public float Accuratezza() => 0.95f;
}
public class DifficoltaDifficile : IDifficolta
{
    private GestoreColpi gestoreColpi;

    public DifficoltaDifficile(GestoreColpi gestoreColpi)
    {
        this.gestoreColpi = gestoreColpi;
    }

    public float CalcolaVelocita() => 40f; // 30f + 10f
    public float ProbabilitaErrore() => 0.1f;
    public float ProbabilitaMiraFuori() => 0.1f;
    public float Accuratezza() => 0.9f;
}
