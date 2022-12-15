using SerialCommunication.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunication.Concrete
{
    public class ModbusRTU_ASCII : IModbus
    {
        private bool[] booleanResponse;

        public byte SlaveAddress { get; set; }

        public SerialPortCommunication _serialPortCommunication { get; set; }

        public ModbusRTU_ASCII (SerialPortCommunication serialPortCommunication, byte slaveAddress) { 
            this._serialPortCommunication = serialPortCommunication;
            SlaveAddress=slaveAddress;  
        }

        public BitArray ReadCoils(int address, int numberOfCoils)
        {
            try
            {
                byte functionCode = (byte)0x01;
                byte[] message = GenerateRequestMessage(functionCode, address, numberOfCoils);
                if (!SendRequest(message))
                    return null;
                byte[] response;
                if (!ReceiveResponse(out response))
                    return null;
                response=response.Skip(3).Take(response[2]).ToArray();
                return new BitArray(response);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        public BitArray ReadDiscreteInputs(int address, int numberOfDiscreteInouts)
        {
            try
            {
                byte functionCode = (byte)0x02;
                byte[] message = GenerateRequestMessage(functionCode, address, numberOfDiscreteInouts);
                if (!SendRequest(message))
                    return null;
                byte[] response;
                if (!ReceiveResponse(out response))
                    return null;
                response = response.Skip(3).Take(response[2]).ToArray();
                return new BitArray(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public byte[] ReadHoldingRegiseters(int address, int numberOfHoldingRegisters)
        {
            try
            {
                byte functionCode = (byte)0x03;
                byte[] message = GenerateRequestMessage(functionCode, address, numberOfHoldingRegisters);
                if (!SendRequest(message))
                    return null;
                byte[] response;
                if (!ReceiveResponse(out response))
                    return null;
                response = response.Skip(3).Take(response[2]).ToArray();
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public byte[] ReadInputRegiseters(int address, int numberOfInputRegiseters)
        {
            try
            {
                byte functionCode = (byte)0x04;
                byte[] message = GenerateRequestMessage(functionCode, address, numberOfInputRegiseters);
                if (!SendRequest(message))
                    return null;
                byte[] response;
                if (!ReceiveResponse(out response))
                    return null;
                response = response.Skip(3).Take(response[2]).ToArray();
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string WriteMultipleRegisters(int address, int registerCount, byte count, byte[] values)
        {
            try
            {
                byte functionCode = (byte)0x10;
                byte[] message = GenerateRequestMessage(functionCode, address, registerCount, count, values);
                if (!SendRequest(message))
                    return null;
                byte[] response;
                if (!ReceiveResponse(out response))
                    return null;
                
                return "Successfully wrote registers and Response Message is :"+response.Select(e=>e+" ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string WriteSingleCoil(int address, int value)
        {
            try
            {
                byte functionCode = (byte)0x05;
                byte[] message = GenerateRequestMessage(functionCode, address, value);
                if (!SendRequest(message))
                    return null;
                byte[] response;
                if (!ReceiveResponse(out response))
                    return null;
                return "Successfully wrote coil and Response Message is :" + response.Select(e => e + " ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string WriteSingleRegister(int address, int value)
        {
            try
            {
                byte functionCode = (byte)0x06;
                byte[] message = GenerateRequestMessage(functionCode, address, value);
                if (!SendRequest(message))
                    return null;
                byte[] response;
                if (!ReceiveResponse(out response))
                    return null;
                return "Successfully wrote coil and Response Message is :" + response.Select(e => e + " ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        private byte[] GenerateRequestMessage(byte FunctionCode, int address, int registerCount, byte count, byte[] values) {

            byte adressHI = BitConverter.GetBytes(address)[1];
            byte adressLO = BitConverter.GetBytes(address)[0];
            byte registerCountHI = BitConverter.GetBytes(registerCount)[1];
            byte registerCountLO = BitConverter.GetBytes(registerCount)[0];
            byte [] Message = new byte[9 + values.Length];

            Message[0] = SlaveAddress;
            Message[1] = FunctionCode;
            Message[2] = adressHI;
            Message[3] = adressLO;
            Message[4] = registerCountHI;
            Message[5] = registerCountLO;
            Message[6] = count;
            for (int i = 0; i < values.Length; i++)
            {
                Message[7 + 2 * i] = BitConverter.GetBytes(values[i])[1];
                Message[7 + 2 * i + 1] = BitConverter.GetBytes(values[i])[0];
            }

            byte[] CRC = GenerateCRC(Message.Take(7+ values.Length).ToArray());
            Message[8 + values.Length] = CRC[0];
            Message[9 + values.Length] = CRC[1];
            return Message;
        }

        private byte[] GenerateRequestMessage(byte functionCode, int address,int number)
        {
            byte [] Message = new byte[8];

            byte adressHI = BitConverter.GetBytes(address)[1];
            byte adressLO = BitConverter.GetBytes(address)[0];
            byte numberHI = BitConverter.GetBytes(number)[1];
            byte numberLO = BitConverter.GetBytes(number)[0];

            Message[0] = SlaveAddress;
            Message[1] = functionCode;
            Message[2] = adressHI;
            Message[3] = adressLO;
            Message[4] = numberHI;
            Message[5] = numberLO;
            byte[] CRC = GenerateCRC(Message.Take(5).ToArray());
            Message[6] = CRC[0];
            Message[7] = CRC[1];

            return Message;
        }

        private static byte[] GenerateCRC(byte[] ProtocolDataUnit)
        {
            ushort crc = 0XFFFF;

            for (int i = 0; i < ProtocolDataUnit.Length; i++)
            {
                crc ^= ProtocolDataUnit[i];

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return BitConverter.GetBytes(crc);
        }

        private bool SendRequest(byte[] message)
        {
            return _serialPortCommunication.SendRequest(message);
        }

        private bool ReceiveResponse(out byte[] response) {
            try
            {
                Thread.Sleep(50);
                bool status=_serialPortCommunication.ReceiveResponse(out response);
                if(!status)
                    return status;
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                response = null;
                return false;
            }


        }

    }
}
