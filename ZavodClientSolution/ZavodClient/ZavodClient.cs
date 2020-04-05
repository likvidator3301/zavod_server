using System.Net.Http;

namespace ZavodClient
{
    public class ZavodClient
    {
        public static HttpClient Client;
        private static string baseUrl;

        public Unit Unit;
        public User User;
        public Session Session;
        public Bag Bag;
        
        public ZavodClient(string url)
        {
            Client = new HttpClient();
            baseUrl = url;
            Unit = new Unit(baseUrl);
            User = new User(baseUrl);
            Session = new Session(baseUrl);
            Bag = new Bag(baseUrl);
        }
    }
}
