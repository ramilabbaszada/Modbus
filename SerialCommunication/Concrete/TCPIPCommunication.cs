using Core.Results.Abstract;
using Core.Results.Concrete;
using SerialCommunication.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunication.Concrete
{
    public class TCPIPClientCommunication : IClientCommunication
    {
        private TcpClient client;
        private NetworkStream stream;
        private byte[] receiveBuffer;
        private string _serverIP;
        private int _serverPort;

        public TCPIPClientCommunication(string serverIP, int serverPort) 
        {
            client = new TcpClient();
            _serverIP = serverIP;
            _serverPort = serverPort;
            receiveBuffer = new byte[261];
        }

        public IDataResult<byte[]> Receive()
        {
            try
            {
                int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                byte[] receivedData = new byte[bytesRead];
                Array.Copy(receiveBuffer, receivedData, bytesRead);
                return new SuccessDataResult<byte[]>( receivedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving data from server: " + ex.Message);
                return new ErrorDataResult<byte[]>(ex.Message);
            }
        }

        public IResult Send(byte[] data)
        {
            try
            {
                if (stream != null)
                {
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Sent data to server.");

                    foreach (byte b in data)
                        Console.Write(b + " ");
                    Console.WriteLine();
                    
                    return new SuccessResult();
                }
                Console.WriteLine("Stream references to null");
                return new ErrorResult("Stream references to null");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending data to server: " + ex.Message);
                return new ErrorResult(ex.Message);
            }
        }

        public IResult Start()
        {
            try
            {
                client.Connect(IPAddress.Parse(_serverIP), _serverPort);
                stream = client.GetStream();
                Console.WriteLine("Connected to server.");
                return new SuccessResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to server: " + ex.Message);
                return new ErrorResult(ex.Message);
            }
        }

        public IResult Stop()
        {
            if (client != null)
            {
                stream.Close();
                client.Close();
                Console.WriteLine("Disconnected from server.");
            }
            return new SuccessResult();
        }
    }
}
