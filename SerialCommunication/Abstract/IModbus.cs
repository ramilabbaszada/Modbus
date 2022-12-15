using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunication.Abstract
{
    public interface IModbus
    {
        public byte SlaveAddress { get; set; }
        public BitArray ReadCoils(int address, int number);
        public BitArray ReadDiscreteInputs(int address, int number);
        public byte[] ReadHoldingRegiseters(int address, int number);
        public byte[] ReadInputRegiseters(int address, int number);
        public string WriteSingleCoil(int address, int value);
        public string WriteSingleRegister(int address, int value);
        public string WriteMultipleRegisters(int address, int registerCount, byte count, byte[] values);
    }
}
