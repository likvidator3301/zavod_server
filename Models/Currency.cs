namespace Models
{
    public class Currency
    {
        public CurrencyType Type { get; set; }
        public int Cost { get; set; }
    }

    public enum CurrencyType
    {
        Dollars,
        Seeds
    }
}