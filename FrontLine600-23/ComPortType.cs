using System.IO.Ports;

namespace FrontLine600_23
{
    public class ComPortType
    {
        public string ComName { get; set; }
        public int BaudRate { get; set; }
        public StopBits StopBits { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public byte Eof { get; set; }
    }
}
