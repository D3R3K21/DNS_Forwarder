using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNS_Forwarder
{
    public class ConsulProperties
    {
        public string Id { get; set; }
        public int Port { get; set; }
        public string Service { get; set; }
        public string Address { get; set; }
    }
}
