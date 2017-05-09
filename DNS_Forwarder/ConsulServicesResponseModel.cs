using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNS_Forwarder
{
    public class ConsulServicesResponseModel
    {
        public ConsulNode Node { get; set; }
        public Dictionary<string, ConsulProperties> Services { get; set; }
    }
}
