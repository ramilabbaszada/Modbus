using Core.Results.Abstract;
using System.Collections;

namespace SerialCommunication.Abstract
{
    public interface IModbus
    {
        public IDataResult<BitArray> ReadCoils(ushort startingAddress, ushort numberOfCoils);
        public IDataResult<BitArray> ReadDiscreteInputs(ushort startingAddress, ushort numberOfDiscreteInputs);
        public IDataResult<byte[]> ReadHoldingRegiseters(ushort startingAddress, ushort numberOfHoldingRegisters);
        public IDataResult<byte[]> ReadInputRegiseters(ushort startingAddress, ushort numberOfInputRegisters);
        public IResult WriteSingleCoil(ushort address, bool value);
        public IResult WriteSingleRegister(ushort address, ushort value);
        public IResult WriteMultipleRegisters(ushort startingAddress, ushort registerCount, byte[] values);
    }
}
