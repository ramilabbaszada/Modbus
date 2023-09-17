using Core.Results.Abstract;
using Core.Results.Concrete;
using SerialCommunication.Abstract;
using SerialCommunication.Constants;
using System.Collections;
using System.Net;


namespace SerialCommunication.Concrete
{
    public class ModbusRTU : IModbus
    {
        public byte SlaveAddress { get; set; }
        public ISerialPortCommunication SerialPortCommunication { get; set; }

        public ModbusRTU (ISerialPortCommunication serialPortCommunication, byte slaveAddress) { 
            this.SerialPortCommunication = serialPortCommunication;
            SlaveAddress=slaveAddress;  
        }

        public IDataResult<BitArray> ReadCoils(ushort startingAddress, ushort numberOfCoils)
        {
            return ReadBooleanMessage(ModbusFunction.ReadCoils, startingAddress, numberOfCoils);
        }

        public IDataResult<BitArray> ReadDiscreteInputs(ushort startingAddress, ushort numberOfDiscreteInouts)
        {
            return ReadBooleanMessage(ModbusFunction.ReadDiscreteInputs, startingAddress, numberOfDiscreteInouts); 
        }

        private IDataResult<BitArray> ReadBooleanMessage(ModbusFunction modbusFunction, ushort startingAddress, ushort number) {

            IDataResult<byte[]> result = SendAndReceiveMessage(modbusFunction, startingAddress, number);

            if (!result.IsSuccess)
                return new ErrorDataResult<BitArray>(result.Message);

            return new SuccessDataResult<BitArray>(new BitArray(result.Data));
        }

        private IDataResult<byte[]> SendAndReceiveMessage(ModbusFunction modbusFunction, ushort startingAddress, ushort number) {
            byte[] message = GenerateRequestMessage(modbusFunction, startingAddress, number);
            IResult result = SendRequest(message);
            if (!result.IsSuccess)
                return new ErrorDataResult<byte[]>(result.Message);

            IDataResult<byte[]> response = ReceiveResponse();
            if (!response.IsSuccess)
                return new ErrorDataResult<byte[]>(response.Message);

            return new SuccessDataResult<byte[]>(response.Data);
        }

        public IDataResult<byte[]> ReadHoldingRegiseters(ushort startingAddress, ushort numberOfHoldingRegisters)
        {
            IDataResult<byte[]> result = SendAndReceiveMessage(ModbusFunction.ReadArchives, startingAddress, numberOfHoldingRegisters);
            if (!result.IsSuccess) return result;

            byte[] mainValues = result.Data.Skip(3).Take(result.Data[2]).ToArray();

            return new SuccessDataResult<byte[]>(mainValues);
        }

        public IDataResult<byte[]> ReadInputRegiseters(ushort startingAddress, ushort numberOfInputRegiseters)
        {
            IDataResult<byte[]> result = SendAndReceiveMessage(ModbusFunction.ReadInputRegiseters, startingAddress, numberOfInputRegiseters);
            if (!result.IsSuccess) return result;

            byte[] mainValues = result.Data.Skip(3).Take(result.Data[2]).ToArray();

            return new SuccessDataResult<byte[]>(mainValues);
        }

        public IResult WriteMultipleRegisters(ushort startingAddress, ushort registerCount,  byte[] values)
        {
                byte[] message = GenerateRequestMessage(ModbusFunction.WriteMultipleRegisters, startingAddress, registerCount, values);
                
                IResult result = SendRequest(message);
                if (!result.IsSuccess)
                    return new ErrorResult(result.Message);

                IResult response = ReceiveResponse();
                if (!response.IsSuccess)
                    return new ErrorResult(response.Message);

                return new SuccessResult();
        }

        public IResult WriteSingleCoil(ushort startingAddress, bool value)
        {
            IDataResult<byte[]> result;

            if (value)
                result = SendAndReceiveMessage(ModbusFunction.WriteSingleCoil, startingAddress, 0xFF00);
            else
                result = SendAndReceiveMessage(ModbusFunction.WriteSingleCoil, startingAddress, 0x0000);

            return result;
        }

        public IResult WriteSingleRegister(ushort startingAddress, ushort value)
        {
            IDataResult<byte[]> result = SendAndReceiveMessage(ModbusFunction.WriteSingleCoil, startingAddress, value);
            return result;
        }
        private byte[] GenerateRequestMessage(ModbusFunction functionCode, ushort address, ushort registerCount, byte[] values) {

            byte adressHI = BitConverter.GetBytes(address)[1];
            byte adressLO = BitConverter.GetBytes(address)[0];
            byte registerCountHI = BitConverter.GetBytes(registerCount)[1];
            byte registerCountLO = BitConverter.GetBytes(registerCount)[0];
            byte [] Message = new byte[9 + values.Length];

            Message[0] = SlaveAddress;
            Message[1] = (byte)functionCode;
            Message[2] = adressHI;
            Message[3] = adressLO;
            Message[4] = registerCountHI;
            Message[5] = registerCountLO;
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

        private byte[] GenerateRequestMessage(ModbusFunction functionCode, ushort address,ushort number)
        {
            byte [] Message = new byte[8];

            byte adressHI = BitConverter.GetBytes(address)[1];
            byte adressLO = BitConverter.GetBytes(address)[0];
            byte numberHI = BitConverter.GetBytes(number)[1];
            byte numberLO = BitConverter.GetBytes(number)[0];

            Message[0] = SlaveAddress;
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

        private IResult SendRequest(byte[] message)
        {
            IResult result=SerialPortCommunication.SendRequest(message);
            return result;
        }

        private IDataResult<byte[]> ReceiveResponse() {
            Thread.Sleep(150);
            IDataResult<byte[]> response=SerialPortCommunication.ReceiveResponse();
            return response; 
        }

    }
}
