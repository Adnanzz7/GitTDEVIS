public static class SalesStats
{
    public static int Buyers;
    public static int SoldWortel;
    public static int SoldTomat;
    public static int SoldKentang;
    public static int SoldCabai;
    public static int CoinEarned;

    public static void Reset()
        => Buyers = SoldWortel = SoldTomat =
           SoldKentang = SoldCabai = CoinEarned = 0;
}
