using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunication.Constants
{
    public enum ModbusFunction
    {
        ReadCoils=0x01, ReadDiscreteInputs= 0x02, ReadHoldingRegiseters=0x03, ReadInputRegiseters=0x04, WriteSingleCoil = 0x05, WriteSingleRegister = 0x06, WriteMultipleRegisters =0x10, ReadArchives=0x68
    }
}
