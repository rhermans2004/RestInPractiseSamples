using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Restbucks
{
    [XmlRoot(Namespace = "http://restbucks.com")]
    [XmlType(TypeName = "order")]
    public class Order
    {
        public Order()
        {
            ConsumeLocation = Location.takeAway;
            OrderStatus = Status.preparing;
            Item i1 = new Item { Name = Item.Coffee.latte, DrinkSize = Item.Size.small, MilkType = Item.Milk.whole, Quantity = 2 };
            Item i2 = new Item { Name = Item.Coffee.cappuccino, DrinkSize = Item.Size.large, MilkType = Item.Milk.skim, Quantity = 1 };
            Items = new List<Item> { i1, i2 };
        }

        public enum Location { takeAway, drinkIn };
        public enum Status { preparing, awaitingPayment, served };

        public string Id
        {
            get;
            set;
        }

        [XmlElement(ElementName = "location")]
        public Location ConsumeLocation
        {
            get;
            set;
        }

        [XmlElement(ElementName = "items")]
        public List<Item> Items
        {
            get;
            set;
        }

        [XmlElement(ElementName = "status")]
        public Status OrderStatus
        {
            get;
            set;
        }
    }
}
