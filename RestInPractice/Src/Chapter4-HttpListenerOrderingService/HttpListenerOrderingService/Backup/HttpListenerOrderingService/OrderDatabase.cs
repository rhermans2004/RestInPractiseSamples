namespace Restbucks
{
    public class OrderDatabase : System.Collections.Generic.Dictionary<string, Order>
    {
        // For testing only!
        static OrderDatabase()
        {
            theDatabase.Add("1234", new Order());
        }

        private static int orderCounter = 0;
        private static OrderDatabase theDatabase = new OrderDatabase();

        public static OrderDatabase Database
        {
            get
            {
                return theDatabase;
            }
        }

        public string Save(Order order)
        {
            string id = orderCounter.ToString();
            Save(id, order);
            orderCounter++;
            return id;
        }

        public void Save(string id, Order order)
        {
            this.Add(id, order);
        }

        public bool Exists(string id)
        {
            return this.ContainsKey(id);
        }

        public Order GetOrder(string id)
        {
            if (this.ContainsKey(id))
            {
                return this[id];
            }
            else
            {
                return null;
            }
        }
    }
}