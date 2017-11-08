using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Text;

namespace Restbucks
{
    public class Order
    {
        public Order()
        {
            location = Location.takeAway;
            status = Status.served;
            Item i1 = new Item { Name = Item.Coffee.latte, DrinkSize = Item.Size.small, MilkType = Item.Milk.whole, Quantity = 2 };
            Item i2 = new Item { Name = Item.Coffee.cappuccino, DrinkSize = Item.Size.large, MilkType = Item.Milk.skim, Quantity = 1 };
            items = new List<Item> { i1, i2 };
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<order xmlns=\"http://schemas.restbucks.com/order\">");
            sb.AppendLine("\t<location>" + location.ToString() + "</location>");
            sb.AppendLine("\t<items>");
            foreach(Item i in items)
            {
                sb.AppendLine("\t\t<item>");
                sb.AppendLine("\t\t\t<name>" + i.Name.ToString() + "</name>");
                sb.AppendLine("\t\t\t<milk>" + i.MilkType.ToString() + "</milk>");
                sb.AppendLine("\t\t\t<size>" + i.DrinkSize.ToString() + "</size>");
                sb.AppendLine("\t\t\t<quantity>" + i.Quantity + "</quantity>");
                sb.AppendLine("\t\t</item>");
            }
            sb.AppendLine("\t</items>");
            sb.AppendLine("\t<status>" + OrderStatus.ToString() + "</status>");
            sb.AppendLine("</order>");
            return sb.ToString();
        }

        string id;
        Location location;
        Status status;
        System.Collections.Generic.List<Item> items;

        public enum Location { takeAway, drinkIn };
        public enum Status { ordered, preparing, awaitingPayment, served };

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public Location ConsumeLocation
        {
            get { return location; }
            set { location = value; }
        }

        public List<Item> Items
        {
            get { return items; }
            set { items = value; }
        }

        public Status OrderStatus
        {
            get { return status; }
            set { status = value; }
        }
    }
}
