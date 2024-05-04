

using SerialCommunication.Concrete;

string portname=SerialPortCommunication.ListAllAvailableSerialPortsAndSelectOne().Data;

ModbusRTU_Master modbusRTU_Master = new ModbusRTU_Master(new SerialPortCommunication(portname),1);

modbusRTU_Master.ReadHoldingRegiseters(1, 10);

Console.WriteLine();
