using Core.Results.Abstract;

namespace SerialCommunication.Abstract
{
    public interface IClientCommunication
    {
        public IResult Start();
        public IResult Stop();
        public IResult Send(byte[] message);
        public IDataResult<byte[]> Receive();
    }
}
