using System.Xml;
using System.Xml.Serialization;

namespace Restbucks
{
    [XmlType(Namespace = "http://restbucks.com", TypeName = "item")]
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

        [XmlElement(ElementName = "name")]
        public Coffee Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlElement(ElementName = "quantity")]
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [XmlElement(ElementName = "milk")]
        public Milk MilkType
        {
            get { return milk; }
            set { milk = value; }
        }

        [XmlElement(ElementName = "size")]
        public Size DrinkSize
        {
            get { return size; }
            set { size = value; }
        }
    }
}
