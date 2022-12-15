using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunication.Abstract
{
    public interface ISerialPortCommunication
    {
        public bool StartCommunication();
        public bool StopCommunication();
        public List<string> ListAllAvailableSerialPorts();
        public bool SendRequest(byte[] message);
        public bool ReceiveResponse(out byte[] Response);
    }
}
