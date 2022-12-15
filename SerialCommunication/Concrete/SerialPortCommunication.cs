using SerialCommunication.Abstract;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunication.Concrete
{
    public class SerialPortCommunication : ISerialPortCommunication
    {
        private string _portName;
        private SerialPort? SelectedPort { get; set; }
        private int Baudrate { get; set;}
        private int Databits { get; set; }
        private StopBits Stopbits { get; set;}
        private Parity ParityBits { get; set;}
        private Handshake Handshake { get; set;}
        public SerialPortCommunication(string portName,int Baudrate,int Databits, StopBits StopBits, Parity parity) {
            this._portName = portName;
            this.Baudrate = Baudrate;
            this.Databits = Databits;
            this.Stopbits = Stopbits;
            this.ParityBits = ParityBits;
            this.Handshake = Handshake.None;
        }
        public bool SendRequest(byte[] message)
        {
            try
            {
                SelectedPort.Write(message, 0, message.Length);
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
            
        }
        public bool ReceiveResponse(out byte[] Response)
        {
            try {
                int bytes = SelectedPort.BytesToRead;
                Response = new byte[bytes];
                SelectedPort.Read(Response, 0, bytes);
                return true;
            }catch (Exception e) {
                Console.WriteLine(e.Message);
                Response = null;
                return false;
            }
        }
        public bool StopCommunication()
        {
            try
            {
                SelectedPort.Close();
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
            
        }
        public bool StartCommunication() {
            try
            {
                SelectedPort = new SerialPort();
                SelectedPort.PortName = _portName;
                SelectedPort.BaudRate = Baudrate;
                SelectedPort.DataBits = Databits;
                SelectedPort.StopBits = Stopbits;
                SelectedPort.Parity = ParityBits;
                SelectedPort.Handshake = Handshake.None;
                SelectedPort.Open();

                if(!SelectedPort.IsOpen)
                {
                    Console.WriteLine( "Port " + SelectedPort.PortName + " can not be opened");
                }
                return true; 
            }
            catch (Exception e) {
                Console.WriteLine( e.Message);  
                return false;
            }
        }
        public List<string> ListAllAvailableSerialPorts()
        {
            try
            {
                String[] ports = SerialPort.GetPortNames();
                Console.WriteLine("Available Serial Ports : ");
                foreach (String portName in ports)
                {
                    Console.WriteLine(portName);
                }

                return ports.ToList();
            }
            catch (Exception e) { 
                Console.WriteLine (e.Message);
                return null;
            }
        }
    }
}
