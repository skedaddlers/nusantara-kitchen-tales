public static class GameData
{
    private static PulauDataSO pulauDipilih;
    private static ResepDataSO resepDipilih;
    private static GameType gameType = GameType.None;

    public static PulauDataSO PulauDipilih
    {
        get => pulauDipilih;
        set
        {
            pulauDipilih = value;
        }
    }

    public static ResepDataSO ResepDipilih
    {
        get => resepDipilih;
        set
        {
            resepDipilih = value;
        }
    }

    public static GameType GameType
    {
        get => gameType;
        set
        {
            gameType = value;
        }
    }

    public static void ResetData()
    {
        PulauDipilih = null;
        ResepDipilih = null;
        GameType = GameType.None;
    }
    
}