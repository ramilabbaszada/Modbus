using Core.Results.Abstract;
using Core.Results.Concrete;
using SerialCommunication.Abstract;
using SerialCommunication.Constants;
using System.Collections;
using System.Net;


namespace SerialCommunication.Concrete
{
    public class ModbusRTU_Master : IModbusMaster
    {
        private byte _slaveAddress;
        private IClientCommunication _communication;
        public ModbusRTU_Master (IClientCommunication Communication, byte slaveAddress) 
        { 
            _communication = Communication;
            _slaveAddress=slaveAddress;
            _communication.Start();
        }
        public IDataResult<byte[]> ReadCoils(ushort startingAddress, ushort numberOfCoils)
        {
            byte[] message = GenerateModbusReadRequestMessage(ModbusFunction.ReadCoils, startingAddress, numberOfCoils);
            IDataResult<byte[]> result = SendAndReceiveMessage(message);

            if (!result.IsSuccess)
                return new ErrorDataResult<byte[]>(result.Message);

            return new SuccessDataResult<byte[]>(result.Data);
        }
        public IDataResult<byte[]> ReadDiscreteInputs(ushort startingAddress, ushort numberOfDiscreteInouts)
        {
            byte[] message = GenerateModbusReadRequestMessage(ModbusFunction.ReadDiscreteInputs, startingAddress, numberOfDiscreteInouts);
            IDataResult<byte[]> result = SendAndReceiveMessage(message);

            if (!result.IsSuccess)
                return new ErrorDataResult<byte[]>(result.Message);

            return new SuccessDataResult<byte[]>(result.Data);
        }
        public IDataResult<byte[]> ReadHoldingRegiseters(ushort startingAddress, ushort numberOfHoldingRegisters)
        {
            byte[] message = GenerateModbusReadRequestMessage(ModbusFunction.ReadHoldingRegiseters, startingAddress, numberOfHoldingRegisters);
            IDataResult<byte[]> result = SendAndReceiveMessage(message);

            if (!result.IsSuccess) 
                return result;

            return new SuccessDataResult<byte[]>(result.Data);
        }
        public IDataResult<byte[]> ReadInputRegiseters(ushort startingAddress, ushort numberOfInputRegiseters)
        {
            byte[] message = GenerateModbusReadRequestMessage(ModbusFunction.ReadInputRegiseters, startingAddress, numberOfInputRegiseters);
            IDataResult<byte[]> result = SendAndReceiveMessage(message);

            if (!result.IsSuccess) 
                return result;

            return new SuccessDataResult<byte[]>(result.Data);
        }
        public IDataResult<byte[]> ReadArchiveRegisters(ushort startingAddress, ushort numberOfArchiveRegisters)
        {
            byte[] message = GenerateModbusReadRequestMessage(ModbusFunction.ReadArchiveRegisters, startingAddress, numberOfArchiveRegisters);
            IDataResult<byte[]> result = SendAndReceiveMessage(message);

            if (!result.IsSuccess)
                return result;

            return new SuccessDataResult<byte[]>(result.Data);
        }
        public IDataResult<byte[]> WriteMultipleRegisters(ushort startingAddress, ushort count, byte[] values)
        {
            byte[] message = GenerateModbusWriteRequestMessage(ModbusFunction.WriteMultipleRegisters, startingAddress,count,values);

            IDataResult<byte[]> result = SendAndReceiveMessage(message);

            if (!result.IsSuccess) 
                return result;

            return new SuccessDataResult<byte[]>(result.Data);
        }
        public IDataResult<byte[]> WriteMultipleCoils(ushort startingAddress, ushort count, bool[] values)
        {

            byte[] message = GenerateModbusWriteRequestMessage(ModbusFunction.WriteMultipleCoils, startingAddress, count, ConvertBoolArrayToByteArray(values));

            IDataResult<byte[]> result = SendAndReceiveMessage(message);

            if (!result.IsSuccess)
                return result;

            return new SuccessDataResult<byte[]>(result.Data);
        }
        public IDataResult<byte[]> WriteSingleCoil(ushort startingAddress, bool value)
        {
            byte[] message;

            if (value)
                message = GenerateModbusReadRequestMessage(ModbusFunction.WriteSingleCoil, startingAddress, 0xFF00);
            else
                message = GenerateModbusReadRequestMessage(ModbusFunction.WriteSingleCoil, startingAddress, 0x0000);

            IDataResult<byte[]> result = SendAndReceiveMessage(message);
            return result;
        }
        public IDataResult<byte[]> WriteSingleRegister(ushort startingAddress, ushort value)
        {
            byte[] message = GenerateModbusReadRequestMessage(ModbusFunction.WriteSingleRegister, startingAddress, value);
            IDataResult<byte[]> result = SendAndReceiveMessage(message);
            return result;
        }
        private byte[] GenerateModbusWriteRequestMessage(ModbusFunction functionCode, ushort startingAddress, ushort count,  byte[] values) {

            byte startingAddressHI = BitConverter.GetBytes(startingAddress)[1];
            byte startingAddressLO = BitConverter.GetBytes(startingAddress)[0];
            byte countHI = BitConverter.GetBytes(count)[1];
            byte countLO = BitConverter.GetBytes(count)[0];
            byte [] Message = new byte[9 + values.Length];

            Message[0] = _slaveAddress;
            Message[1] = (byte)functionCode;
            Message[2] = startingAddressHI;
            Message[3] = startingAddressLO;
            Message[4] = countHI;
            Message[5] = countLO;
            Message[6] = (byte)values.Length;
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
        private byte[] GenerateModbusReadRequestMessage(ModbusFunction functionCode, ushort address,ushort number)
        {
            byte [] Message = new byte[8];

            byte adressHI = BitConverter.GetBytes(address)[1];
            byte adressLO = BitConverter.GetBytes(address)[0];
            byte numberHI = BitConverter.GetBytes(number)[1];
            byte numberLO = BitConverter.GetBytes(number)[0];

            Message[0] = _slaveAddress;
            Message[1] = (byte)functionCode;
            Message[2] = adressHI;
            Message[3] = adressLO;
            Message[4] = numberHI;
            Message[5] = numberLO;

            byte[] CRC = GenerateCRC(Message.Take(6).ToArray());
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
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc >>= 1;
                }
            }

            return BitConverter.GetBytes(crc);
        }
        private IDataResult<byte[]> SendAndReceiveMessage(byte[] message)
        {
            IResult result = SendRequest(message);
            if (!result.IsSuccess)
                return new ErrorDataResult<byte[]>(result.Message);

            IDataResult<byte[]> response = ReceiveResponse();
            if (!response.IsSuccess)
                return new ErrorDataResult<byte[]>(response.Message);

            return new SuccessDataResult<byte[]>(response.Data);
        }
        private IResult SendRequest(byte[] message)
        {
            IResult result = _communication.Send(message);
            return result;
        }
        private IDataResult<byte[]> ReceiveResponse()
        {
            Thread.Sleep(150);
            IDataResult<byte[]> response = _communication.Receive();
            return response;
        }
        private byte[] ConvertBoolArrayToByteArray(bool[] boolArray)
        {
            int bytesLength = (boolArray.Length + 7) / 8;
            byte[] byteArray = new byte[bytesLength];

            for (int i = 0; i < boolArray.Length; i++)
            {
                if (boolArray[i])
                {
                    byteArray[i / 8] |= (byte)(1 << (i % 8));
                }
            }

            return byteArray;
        }

    }
}
