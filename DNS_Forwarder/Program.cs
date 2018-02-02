using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using DNS_Forwarder.Properties;

namespace DNS_Forwarder
{
    class Program
    {
        public static Dictionary<string, ConsulProperties> ConsulServices;

        static void RefreshServices(string endpoint)
        {
            var client = new HttpClient(endpoint);
            var s = client.Get<ConsulServicesResponseModel>();
            ConsulServices = new Dictionary<string, ConsulProperties>();
            if (s?.Services == null)
            {
                Console.Out.WriteLine($"Endpoint not found : {endpoint}");
                return;
            }
            foreach (var g in s.Services)
            {
                var key = g.Value.Service;
                if (!ConsulServices.ContainsKey(key))
                    ConsulServices.Add(key, g.Value);
            }
            Console.Out.WriteLine($"{ConsulServices.Count} Services Loaded From : {endpoint}");
        }

        private static string Node { get; set; }
        private static string _ep;
        private static string Endpoint => $"http://{_ep}:8500/v1/catalog/node/{Node}";

        static void Main(string[] args)
        {
            _ep = Settings.Default.Node + ".dev.corpdomain.local";
            Node = Settings.Default.Node;
            RefreshServices(Endpoint);
            var ip = IPAddress.Parse("127.0.0.2");
            using (DnsServer server = new DnsServer(ip, 10, 10))
            {

                server.QueryReceived += OnQueryReceived;
                server.Start();
                Console.WriteLine("Press q to stop server, or enter new node name for consul");
                var control = true;

                while (control)
                {
                    try
                    {
                        var key = Console.ReadLine()?.ToLower() ?? string.Empty;
                        switch (key)
                        {
                            case "":
                                {
                                    RefreshServices(Endpoint);
                                    break;
                                }
                            case "r":
                                {
                                    Console.Out.WriteLine("Please enter relase environment url");
                                    _ep = Console.ReadLine()?.ToLower() ?? string.Empty;
                                    Console.Out.WriteLine("Please enter consul node");
                                    Node = Console.ReadLine()?.ToLower() ?? string.Empty;
                                    RefreshServices(Endpoint);
                                    break;
                                }
                            case "release":
                                {
                                    Console.Out.WriteLine("Please enter relase environment url");
                                    _ep = Console.ReadLine()?.ToLower() ?? string.Empty;
                                    Console.Out.WriteLine("Please enter consul node");
                                    Node = Console.ReadLine()?.ToLower() ?? string.Empty;
                                    RefreshServices(Endpoint);
                                    break;
                                }
                            case "q":
                                {
                                    control = false;
                                    break;
                                }
                            case "quit":
                                {
                                    control = false;
                                    break;
                                }
                            case "h":
                                {
                                    Console.Out.WriteLine("Enter to refresh currently selected environment");
                                    Console.Out.WriteLine("r/release to use release environment");
                                    Console.Out.WriteLine("q/quit to exit");
                                    Console.Out.WriteLine("h/help to display usage");
                                    break;
                                }
                            case "help":
                                {

                                    break;
                                }
                            default:
                                {
                                    _ep = key + ".dev.corpdomain.local";
                                    Node = key;
                                    try
                                    {
                                        RefreshServices(Endpoint);
                                    }
                                    catch (Exception e)
                                    {
                                        Node = Settings.Default.Node;
                                        RefreshServices(Endpoint);
                                    }
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR: " + e);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }
        static async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
        {
            DnsMessage query = e.Query as DnsMessage;

            DnsMessage response = query?.CreateResponseInstance();

            if (query?.Questions.Count == 1)
            {
                DnsQuestion question = query.Questions[0];
                ConsulProperties consuleService = null;
                var serviceName = question.Name.Labels[0];

                if (ConsulServices.TryGetValue(serviceName, out consuleService))
                {
                    var test = DomainName.Parse(_ep);

                    var record = new SrvRecord(DomainName.Parse(consuleService.Service), 10000, 1, 1,
                        (ushort)consuleService.Port, test);
                    response.AnswerRecords.Add(record);
                    response.ReturnCode = ReturnCode.NoError;
                    e.Response = response;
                }
            }
        }
    }
}

