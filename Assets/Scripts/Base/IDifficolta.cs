// Gestiamo la difficoltà con il Pattern Strategy

public interface IDifficolta
{
    float CalcolaVelocita();
    float ProbabilitaErrore();
    float ProbabilitaMiraFuori();
    float Accuratezza();
}
