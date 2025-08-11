using Unity.VisualScripting;

public class Punteggio
{
    private int punti;

    public void AggiungiPunto()
    {
        if (punti == 0) punti = 15;
        else if (punti == 15) punti = 30;
        else if (punti == 30) punti = 40;
        else punti = 50; // Game vinto
    }

    public int GetPunti() => punti;

    public void ResetPunti() => punti = 0;

    public string PunteggioToString()
    {
        if (punti == 0) return "00";
        else if (punti == 15) return "15";
        else if (punti == 30) return "30";
        else if (punti == 40) return "40";
        else return "Game";
    }
}

public class Game
{
    private int gameGiocatore = 0;
    private int gameBot = 0;
    private int puntiPerVittoria;

    public Game(int puntiPerVittoria)
    {
        this.puntiPerVittoria = puntiPerVittoria;
    }

    public void AggiungiGameGiocatore() => gameGiocatore++;
    public void AggiungiGameBot() => gameBot++;
    public bool HaVintoGiocatore() => gameGiocatore >= puntiPerVittoria;
    public bool HaVintoBot() => gameBot >= puntiPerVittoria;
    public int GetGameGiocatore() => gameGiocatore;
    public int GetGameBot() => gameBot;

    public string GameToString(int game)
    {
        if (game == 0) return "0";
        return game.ToString();
    }
}
