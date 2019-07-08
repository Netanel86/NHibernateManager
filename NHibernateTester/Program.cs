using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate;
using NHibernateTester.Model;
using System.Collections;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;

namespace NHibernateTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = String.Empty;
            Guid id = Guid.Empty;
            //ShoppingCart cart = null;
            //IList<Product> myProducts = null;
            while (input.CompareTo("exit") != 0)
            {

                Console.WriteLine(
@"1 - add cart
2 - add product
3 - print carts
4 - print products
5 - delete cart
exit - close app");
                Console.WriteLine("Enter any key to continue...");
                
                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        id = addCart();
                        printCart(id);
                        break;

                    case "2":
                        id = addProduct();
                        printCart(id);
                        break;
                    
                    case "3":
                        printCarts();
                        break;

                    case "4":
                        printProducts();
                        break;

                    case "5":
                        delete();
                        break;

                    case "6":
                        listit();
                        break;
                }
            }
        }

        private static void listit()
        {
            using (ISession session = Global.InstanceDAO.OpenSession())
            {
                var list = Global.InstanceDAO.GetAllValues<string, Product>(session, "Name", true);
                int count = 1;
                Console.WriteLine("#####################################################################################");
                foreach (string id in list)
                {
                    Console.WriteLine(String.Format("({0}) Product Name: [{1}]\n", count++, id));
                }
                Console.WriteLine("#####################################################################################");
            }
        }

        private static void printProducts()
        {
            using (ISession session = Global.InstanceDAO.OpenSession())
            {
                IList<Product> list = Global.InstanceDAO.GetAll<Product>(session);
                Console.WriteLine("#####################################################################################");
                foreach (Product p in list)
                {
                    Console.WriteLine(p.ToString());
                }
                Console.WriteLine("#####################################################################################");
            }
        }
        private static void printCarts()
        {
            using (ISession session = Global.InstanceDAO.OpenSession())
            {
                IList<ShoppingCart> list = Global.InstanceDAO.GetAll<ShoppingCart>(session);
                
                foreach (ShoppingCart p in list)
                {
                    Console.WriteLine("#####################################################################################");
                    Console.WriteLine(p.ToString());
                    Console.WriteLine("#####################################################################################");
                }
                
            }
        }
        private static Guid addCart()
        {
            ShoppingCart cart = CreateShoppingCart();
            using (ISession session = Global.InstanceDAO.OpenSession())
            {
                Console.WriteLine("#################################### CREATED ########################################");
                Console.WriteLine(cart.ToString());
                Console.WriteLine("#################################### CREATED ########################################");
                Global.InstanceDAO.SaveOrUpdate(session, cart);
                Console.WriteLine("#################################### SAVED ########################################");
                Console.WriteLine(cart.ToString());
                Console.WriteLine("#################################### SAVED ########################################");
            }
         
            return cart.Id;
        }
        private static void printCart(Guid id)
        {
            using (ISession session = Global.InstanceDAO.OpenSession())
            {
                ShoppingCart cart = Global.InstanceDAO.GetById<ShoppingCart>(session, id);
                Console.WriteLine("################################## RELOADED #############################################");
                Console.WriteLine(cart.ToString());
                Console.WriteLine("################################## RELOADED #############################################");
            }
        }
        private static void delete()
        {
            Guid cartID = Guid.Empty;
            ShoppingCart cart = null;

            using (ISession session = Global.InstanceDAO.OpenSession())
            {
                cart = getSelectedCart(Global.InstanceDAO.GetAll<ShoppingCart>(session));
                Global.InstanceDAO.Delete(session, cart);
            }
        }

        private static Guid addProduct()
        {
            Guid cartID = Guid.Empty;
            ShoppingCart cart = null;

            using (ISession session = Global.InstanceDAO.OpenSession())
            {
                cart = getSelectedCart(Global.InstanceDAO.GetAll<ShoppingCart>(session));
                cart.AddProduct(CreateProduct(1001));
                Global.InstanceDAO.SaveOrUpdate<ShoppingCart>(session, cart);
            }

            cartID = cart.Id;
            
            return cartID;
        }

        private static ShoppingCart getSelectedCart(IList<ShoppingCart> carts)
        {
            ShoppingCart cart = null;
            int count = 1;
            Console.WriteLine("#####################################################################################");
            foreach (ShoppingCart p in carts)
            {
                Console.WriteLine(String.Format("({0}) Cart ID: [{1}]\n", count++, p.Id));
            }
            Console.WriteLine("#####################################################################################");
            Console.WriteLine("Enter Cart number...");
            int input = Convert.ToInt32(Console.ReadLine());

            cart = carts.ToList<ShoppingCart>()[input - 1];

            return cart;
        }

        private static ShoppingCart CreateShoppingCart()
        {
            ShoppingCart cart = new ShoppingCart();
            cart.AddProduct(new Product() { Category = "Appliances", Name = "Refrigirator", Price = 15000 });
            cart.AddProduct(new Product() { Category = "Furniture", Name = "Sofa", Price = 5700 });
            cart.AddProduct(new Product() { Category = "Computers", Name = "Laptop", Price = 3700 });
            cart.AddProduct(new Product() { Category = "Cellular", Name = "SmartPhone", Price = 1700 });

            return cart;
        }
        private static Product CreateProduct(int value)
        {
            return new Product() { Category = "Appliances", Name = "Refrigirator", Price = value };
        }
    }
}
