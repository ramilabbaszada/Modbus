using Core.Results.Abstract;
using Core.Results.Concrete;
using SerialCommunication.Abstract;
using System;
using System.IO;
using System.IO.Ports;

namespace SerialCommunication.Concrete
{
    public class SerialPortCommunication : IClientCommunication
    {
        private SerialPort serialPort;
        private byte[] buffer;

        public SerialPortCommunication(string portName, int baudRate=9600, int dataBits=8, StopBits stopBits=StopBits.One, Parity parity= Parity.Even, Handshake handshake=Handshake.None)
        {
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                Handshake = handshake
            };
            serialPort.DataReceived += OnDataReceived;
        }
        public IResult Send(byte[] data)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Write(data, 0, data.Length);
                    Console.WriteLine("Data sent successfully.");
                    return new SuccessResult();
                }
                else
                {
                    Console.WriteLine("Serial port is not open. Cannot send data.");
                    return new ErrorResult("Serial port is not open. Cannot send data.");
                }
            }
            catch (Exception e) {
                return new ErrorResult(e.Message);
            }
        }
        public IDataResult<byte[]> Receive()
        {

            if (serialPort.IsOpen)
            {
                if (buffer == null)
                    return new ErrorDataResult<byte[]>("There is no data to read from buffer");
                else
                    return new SuccessDataResult<byte[]>(buffer);
            }
            else
            {
                Console.WriteLine("Serial port is not open. Cannot receive data.");
                return new ErrorDataResult<byte[]>("\"Serial port is not open. Cannot receive data.\"");
            }

            
   
        }

        public IResult Stop()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    Console.WriteLine("Serial port closed successfully.");
                }
                else
                {
                    Console.WriteLine("Serial port is not open.");
                }
                return new SuccessResult();
            }
            catch (Exception e) {
                return new ErrorResult(e.Message);
            }

        }
        public IResult Start() 
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    Console.WriteLine("Serial port opened successfully.");
                }
                else
                {
                    Console.WriteLine("Serial port is already open.");
                }
                return new SuccessResult();
            }
            catch (Exception ex) 
            {
                return new ErrorResult(ex.Message);
            }
        }

        public static IDataResult<String> ListAllAvailableSerialPortsAndSelectOne()
        {
            String selectedPort;
            try
            {
                string[] portNames = SerialPort.GetPortNames();
                Console.WriteLine("Available serial ports:");
                foreach (var portName in portNames)
                {
                    Console.WriteLine(portName);
                }
                Console.Write("Enter the port name to use: ");
                selectedPort = Console.ReadLine();
                return new SuccessDataResult<String>(selectedPort);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<String>(ex.Message);
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            buffer = new byte[serialPort.BytesToRead];
            int bytesRead = serialPort.Read(buffer, 0, serialPort.BytesToRead);
            Console.WriteLine($"Data Received: {BitConverter.ToString(buffer)}");
        }

    }
}
