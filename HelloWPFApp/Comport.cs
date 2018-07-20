using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

namespace HelloWPFApp
{
    class Comport
    {
        string portNum = "";
        int baudRate = 9600;
        Parity parity = Parity.Even;
        int dataBits = 7;
        StopBits stopBits = StopBits.One;

        private static SerialPort port = new SerialPort();

        public void PortOpen()
        {
            //File.AppendAllText("C:/Users/sasaki.TAROT/Desktop/log.txt", "Start");
            port.PortName = portNum;
            port.BaudRate = baudRate;
            port.Parity = parity;
            port.DataBits = dataBits;
            port.StopBits = stopBits;
            port.DtrEnable = true;
            port.Open();
            port.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

        }

        public void PortClose()
        {
            port.Close();
            port.Dispose();
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            port = (SerialPort)sender;
            byte[] buf = new byte[1024];
            int len = port.Read(buf, 0, 1024);
            string s = Encoding.GetEncoding("Shift_JIS").GetString(buf, 0, len);
            Console.WriteLine(s);
            File.AppendAllText("C:/Users/sasaki.TAROT/Desktop/log.txt", s);
        }

    }
}
