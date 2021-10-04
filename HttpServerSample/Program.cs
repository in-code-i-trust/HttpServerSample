using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Mime;
using System.IO;

namespace HttpServerSample
{
    class Program
    {
        void Listen()
        {
            try
            {
                var listener = new HttpListener();
                //listener.Prefixes.Add("https://*:443/");
                listener.Prefixes.Add("http://*:80/");
                listener.Start();
                while (true)
                {
                    try
                    {
                        var context = listener.GetContext();

                        var req = context.Request;
                        Console.WriteLine($"RawURL: {req.RawUrl.ToString()}");
                        if (req.RawUrl.ToString() != "/")
                        {
                            context.Response.Close();
                            continue;
                        }

                        Console.WriteLine($"URL: {req.Url.ToString()}");
                        Console.WriteLine($"Method: {req.HttpMethod}");
                        Console.WriteLine($"UA: {req.UserAgent}");
                        Console.WriteLine($"HasBody: {req.HasEntityBody}");

                        string body = null;
                        if (req.HasEntityBody)
                        {
                            using (var sr = new StreamReader(req.InputStream))
                            {
                                body = sr.ReadToEnd();
                            }
                        }

                        if (body != null)
                        {
                            Console.WriteLine($"Body is ...");
                            Console.WriteLine($"```");
                            Console.WriteLine($"{body}");
                            Console.WriteLine($"```");
                        }

                        if (body == null)
                        {
                            context.Response.Close();
                            continue;
                        }

                        // 

                        var res = context.Response;

                        res.StatusCode = 200;
                        res.ContentEncoding = Encoding.UTF8;
                        res.ContentType = MediaTypeNames.Text.Plain;

                        var content = Encoding.UTF8.GetBytes("Hi, there.");
                        res.OutputStream.Write(content, 0, content.Length);

                        res.Close();
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine($"Context: {ee.GetType().FullName} {ee.Message}");
                    }
                    finally
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        void Init()
        {
            new Thread(Listen).Start();
        }

        static void Main(string[] args)
        {
            new Program().Init();
        }
    }
}
