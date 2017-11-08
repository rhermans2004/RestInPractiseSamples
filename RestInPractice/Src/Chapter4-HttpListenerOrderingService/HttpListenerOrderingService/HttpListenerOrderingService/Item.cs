using System.Runtime.Serialization;

namespace Restbucks
{
    public class Item
    {

        public enum Coffee { latte, cappuccino, espresso };
        public enum Milk { skim, semi, whole };
        public enum Size { small, medium, large };

        private Coffee name;
        private int quantity;
        private Milk milk;
        private Size size;

        public Item() { }

        public Item(Coffee name, int quantity, Milk milk, Size size)
        {
            this.name = name;
            this.quantity = quantity;
            this.milk = milk;
            this.size = size;
        }

        public Coffee Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public Milk MilkType
        {
            get { return milk; }
            set { milk = value; }
        }

        public Size DrinkSize
        {
            get { return size; }
            set { size = value; }
        }
    }
}
