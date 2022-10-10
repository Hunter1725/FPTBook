using Newtonsoft.Json;
namespace FPTBook.Utils
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class CartItem
    {
        public int ID;
        public string Title;
        public string Poster;
        public string Author;
        public decimal Price;
        public int Quantity;
        public CartItem(int id,string title,string poster,string author, decimal price,int quantity)
        {
            this.ID=id;
            this.Title=title;
            this.Poster=poster;
            this.Author=author;
            this.Price=price;
            this.Quantity=quantity;
        }
    }
    public class ShoppingCart {
        public List<CartItem> Items { get; private set; }
        
        // A protected constructor ensures that an object can't be created from outside
        public ShoppingCart() { 

            this.Items = new List<CartItem>();
                
        }
        /**
        * AddItem() - Adds an item to the shopping
        */        
        public CartItem AddItem(int productId,string title,string poster,string author,decimal price, int quantity) {
            CartItem newItem=new CartItem(productId,title,poster,author,price,quantity) ;
            // If this item already exists in our list of items, increase the quantity
            // Otherwise, add the new item to the list
            bool itemExist=false;
            for (int i=0;i<this.Items.Count;i++) {
                if (this.Items[i].ID==productId) {
                    this.Items[i].Quantity+=quantity;
                    itemExist=true;
                    break;
                }
            }  
            // Create a new item to add to the cart
            if(!itemExist)        
            {
                this.Items.Add(newItem);
                
            }
            return newItem;
        }
        public void EditItem(int productId, int quantity) {            
            foreach (CartItem item in this.Items) {
                if (item.ID==productId) {
                    if(quantity==0)
                        this.Items.Remove(item);
                    else
                        item.Quantity=quantity;                        
                    break;
                }
            }
        }
        public void RemoveItem(int productId) {            
            this.EditItem(productId,0);
        }
    }
    public class ReportItem
    {
        public string Title;
        public int Total;
        public ReportItem(string title,int total)
        {
            Title=title;
            Total=total;
        }
    }
}