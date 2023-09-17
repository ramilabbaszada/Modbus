using Core.Results.Abstract;
using Core.Results.Concrete;
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
        private SerialPort SelectedPort { get; set; }
        private int Baudrate { get; set;}
        private int Databits { get; set; }
        private StopBits _Stopbits { get; set;}
        private Parity ParityBits { get; set;}
        private Handshake _Handshake { get; set;}

        public SerialPortCommunication(string portName,int Baudrate=9600,int Databits=8, StopBits stopBits=StopBits.One, Parity parity=Parity.Even, Handshake handshake=Handshake.None) {
            this._portName = portName;
            this.Baudrate = Baudrate;
            this.Databits = Databits;
            this._Stopbits = stopBits;
            this.ParityBits = parity;
            this._Handshake = handshake;
        }

        public IResult SendRequest(byte[] message)
        {
            try
            {
                SelectedPort.Write(message, 0, message.Length);
                return new SuccessResult();
            }
            catch (Exception e) {
                return new ErrorResult(e.Message);
            }
            
        }
        public IDataResult<byte[]> ReceiveResponse()
        {
            try {
                int bytesToRead = SelectedPort.BytesToRead;
                byte [] Response = new byte[bytesToRead];
                SelectedPort.Read(Response, 0, bytesToRead);
                return new SuccessDataResult<byte[]>(Response);
            }catch (Exception e) {
                return new ErrorDataResult<byte[]>(e.Message);
            }
        }
        public IResult StopCommunication()
        {
            try
            {
                SelectedPort.Close();
                return new SuccessResult();
            }
            catch (Exception e) {
                return new ErrorResult(e.Message);
            }

        }

        public IResult StartCommunication() {
            try
            {
                SelectedPort = new SerialPort();
                SelectedPort.PortName = _portName;
                SelectedPort.BaudRate = Baudrate;
                SelectedPort.DataBits = Databits;
                SelectedPort.StopBits = _Stopbits;
                SelectedPort.Parity = ParityBits;
                SelectedPort.Handshake = _Handshake;
                SelectedPort.Open();

                if(!SelectedPort.IsOpen)
                    return new ErrorResult();

                return new SuccessResult();
            }
            catch (Exception e) {
                return new ErrorResult(e.Message);
            }
        }

        public static IDataResult<List<string>> ListAllAvailableSerialPorts()
        {
            try
            {
                String[] ports = SerialPort.GetPortNames();
                Console.WriteLine("Available Serial Ports : ");
                foreach (String portName in ports)
                {
                    Console.WriteLine(portName);
                }

                return new SuccessDataResult<List<string>>(ports.ToList());
            }
            catch (Exception e)
            {
                return new ErrorDataResult<List<string>>(e.Message);
            }
        }

    }
}
