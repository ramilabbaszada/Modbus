using Core.Results.Abstract;
using System.Collections;

namespace SerialCommunication.Abstract
{
    public interface IModbusMaster
    {
        public IDataResult<byte[]> ReadCoils(ushort startingAddress, ushort numberOfCoils);
        public IDataResult<byte[]> ReadDiscreteInputs(ushort startingAddress, ushort numberOfDiscreteInputs);
        public IDataResult<byte[]> ReadHoldingRegiseters(ushort startingAddress, ushort numberOfHoldingRegisters);
        public IDataResult<byte[]> ReadInputRegiseters(ushort startingAddress, ushort numberOfInputRegisters);
        public IDataResult<byte[]> ReadArchiveRegisters(ushort startingAddress, ushort numberOfArchiveRegisters);
        public IDataResult<byte[]> WriteSingleCoil(ushort address, bool value);
        public IDataResult<byte[]> WriteSingleRegister(ushort address, ushort value);
        public IDataResult<byte[]> WriteMultipleCoils(ushort startingAddress,ushort count, bool[] values);
        public IDataResult<byte[]> WriteMultipleRegisters(ushort startingAddress,ushort count, byte[] values);
    }
}
