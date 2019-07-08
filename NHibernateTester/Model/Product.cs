using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernateTester.Model
{
    public class Product
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Category { get; set; }
        public virtual int Price { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }
       
        public override string ToString()
        {
            return String.Format("\n[Id: {0}]" + Environment.NewLine +
                "[CartId: {1}]" + Environment.NewLine + 
                "Name: {2}"+ Environment.NewLine +
                "Categoty: {3}"+ Environment.NewLine +
                "Price: {4}", Id, ShoppingCart == null ? "NULL" : ShoppingCart.Id.ToString(), Name, Category, Price);
        }
    }
}
