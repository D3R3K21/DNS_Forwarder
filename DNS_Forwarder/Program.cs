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
        static void RefreshServices()
        {
            var endpoint = $"http://{Settings.Default.Node}.dev.corpdomain.local:8500/v1/catalog/node/{Settings.Default.Node}";
            var client = new HttpClient(endpoint);
            var s = client.Get<ConsulServicesResponseModel>();
            ConsulServices = new Dictionary<string, ConsulProperties>();
            foreach (var g in s.Services)
            {
                var key = g.Value.Service;
                if (!ConsulServices.ContainsKey(key))
                    ConsulServices.Add(key, g.Value);
            }
        }

        static void Main(string[] args)
        {
            RefreshServices();
            var ip = IPAddress.Parse("127.0.0.2");
            using (DnsServer server = new DnsServer(ip, 10, 10))
            {
                server.QueryReceived += OnQueryReceived;

                server.Start();

                Console.WriteLine("Press any key to stop server");
                Console.ReadLine();
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
                    var test = DomainName.Parse($"{Settings.Default.Node}.dev.corpdomain.local");

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

