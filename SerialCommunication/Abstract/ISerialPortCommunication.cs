
using Core.Results.Abstract;
using Core.Results.Concrete;
using System.IO.Ports;

namespace SerialCommunication.Abstract
{
    public interface ISerialPortCommunication
    {
        public IResult StartCommunication();
        public IResult StopCommunication();
        public IResult SendRequest(byte[] message);
        public IDataResult<byte[]> ReceiveResponse();
    }
}
