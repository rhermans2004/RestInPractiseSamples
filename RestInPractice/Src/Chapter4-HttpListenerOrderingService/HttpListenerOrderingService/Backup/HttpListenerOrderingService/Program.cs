using System;
using System.Text;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace Restbucks 
{
    public class Program
    {
        static Dictionary<Thread, HttpListenerContext> contexts = new Dictionary<Thread, HttpListenerContext>();

        public delegate void HttpAction(HttpListenerContext context);
        public static event HttpAction Post;
        public static event HttpAction Get;
        public static event HttpAction Put;
        public static event HttpAction Delete;

        static Program()
        {
            Delete += DeleteResource;
            Get += GetResource;
        }

        public static void Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerResponse response = context.Response;
                response.SendChunked = false;
                response.KeepAlive = true;
                
                Thread worker = new Thread(DispatchRequest);
                
                contexts.Add(worker, context);
                worker.Start();
            }
            listener.Stop();
        }

        static void GetResource(HttpListenerContext context)
        {
            string orderId = ExtractOrderId(context.Request);
            Console.WriteLine(orderId);

            if (OrderDatabase.Database.Exists(orderId))
            {
                Order order = OrderDatabase.Database[orderId];
                byte[] response = new ASCIIEncoding().GetBytes(order.ToString());
                context.Response.ContentLength64 = response.Length;
                context.Response.OutputStream.Write(response, 0, response.Length);
            }

            context.Response.Close();
        }

        static void DeleteResource(HttpListenerContext context)
        {
            string orderId = ExtractOrderId(context.Request);

            if (OrderDatabase.Database.Exists(orderId))
            {
                Order order = OrderDatabase.Database[orderId];

                if (order.OrderStatus == Order.Status.ordered)
                {
                    OrderDatabase.Database.Remove(orderId);
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                    return;
                }
                else
                {
                    context.Response.StatusCode = 409;
                    byte[] response = new ASCIIEncoding().GetBytes(order.ToString());
                    context.Response.ContentLength64 = response.Length;
                    context.Response.OutputStream.Write(response, 0, response.Length);
                    context.Response.Close();
                    return;
                }
            }

            context.Response.StatusCode = 404;
            context.Response.Close();
        }

        private static string ExtractOrderId(HttpListenerRequest httpListenerRequest)
        {
            string[] pathParts = httpListenerRequest.Url.Segments;
            return pathParts[pathParts.Length -1];
        }

        public static void DispatchRequest()
        {
            Console.WriteLine(contexts[Thread.CurrentThread].Request.HttpMethod.ToLower());

            switch (contexts[Thread.CurrentThread].Request.HttpMethod.ToLower())
            {
                    
                case "post":
                    {
                        break;
                    }

                case "get":
                    {
                        Get(contexts[Thread.CurrentThread]);
                        break;
                    }
                case "put":
                    {
                        break;
                    }
                case "delete":
                    {
                        Delete(contexts[Thread.CurrentThread]);
                        break;
                    }
                default:
                    {
                        throw new MissingMethodException("The HTTP verb [" + contexts[Thread.CurrentThread].Request.HttpMethod + "] is not supported.");
                    }
            }
        }
    }
}