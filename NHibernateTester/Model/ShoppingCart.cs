using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernateTester.Model
{
    public class ShoppingCart
    {
        public virtual Guid Id { get; set; }

        public virtual ISet<Product> Products { get; set; }

        public ShoppingCart()
        {
            Products = new HashSet<Product>();
        }

        public virtual void AddProduct(Product i_Product)
        {
            Products.Add(i_Product);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder()
                .Append(String.Format("Shopping Cart ID: [{0}]" + Environment.NewLine + "{1} Products in cart: ", Id, Products.Count));
            foreach (Product p in Products)
            {
                builder.Append(Environment.NewLine + p.ToString());
            }
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            ShoppingCart toCompare =obj as ShoppingCart;
            bool isEqual;


            if (isEqual = this.Id.Equals(toCompare.Id) && this.Products.Count == toCompare.Products.Count)
            {
                bool equalProducts = false;
                bool updatedProduct = false;
                IList<Product> myProducts = this.Products.ToList();
                IList<Product> compareProducts = toCompare.Products.ToList();
                
                for (int i = 0; i < this.Products.Count; i++ )
                {
                    equalProducts = myProducts[i].Id.Equals(compareProducts[i].Id);
                    updatedProduct = compareProducts[i].ShoppingCart != null && compareProducts[i].ShoppingCart.Id.Equals(this.Id);
                    if (!equalProducts || !updatedProduct)
                    {
                        isEqual = false;
                        break;
                    }
                }
            }

            return isEqual;
        }
    }
}
