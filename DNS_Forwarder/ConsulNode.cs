using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNS_Forwarder
{
    public class ConsulNode
    {
        public string Node { get; set; }
        public string Address { get; set; }
        public int CreateIndex { get; set; }
        public int ModifyIndex { get; set; }
    }
}
