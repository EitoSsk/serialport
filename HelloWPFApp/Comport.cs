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
        string portNum = "COM5";
        int baudRate = 9600;
        Parity parity = Parity.Even;
        int dataBits = 7;
        StopBits stopBits = StopBits.One;

        #region パブリック変数
        /// <summary>読み込みバッファ</summary>
        private byte[] readBuffer = new byte[1024];

        /// <summary>readBuffer に格納済みのサイズ</summary>
        private int readBufferLength;

        private string mes;
        private string m_TAName = "";

        #endregion

        #region プロパティ
        // 受信メッセージ
        public string BaseMessage
        {
            get { return mes; }
            set { mes = value; }
        }
        // 受信バッファ
        public byte[] ReadBuffer
        {
            get { return readBuffer; }
            set { readBuffer = value; }
        }
        // 受信バッファサイズ
        public int ReadBufferLength
        {
            get { return readBufferLength; }
            set { readBufferLength = value; }
        }
        // TA名
        public string TAName
        {
            get { return m_TAName; }
            set { m_TAName = value; }
        }

        #endregion

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

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            if (this.ReadBufferLength <= 0)
                this.ReadBufferLength = 0;

            // ↓
            int len = port.Read(this.ReadBuffer, this.ReadBufferLength, this.ReadBuffer.Length - this.ReadBufferLength);
            if (len == 0)
                return;

            this.ReadBufferLength += len;
            // ↑

            int count = 0;          //ETXの位置
            int byteIndex = 0;      //読込開始位置
            string mes = "";

            // 取得不能文字エラー回避 mod 2014.04.11 DPLSakazaki
            try
            {

                foreach (byte b in this.ReadBuffer)
                {
                    byte[] reSts = new byte[2];

                    //データが無ければ処理中断
                    if (b == 0)
                        break;

                    count++;


                    //CRLFが来るまで処理を繰り返す
                    //Array.Copy(this.ReadBuffer, byteIndex + count - 1, reSts, 0, 2);
                    //if (!ByteEquals(reSts, sts_CRLF))
                    //    continue;

                    //ETX(0x03)が来るまで処理を繰り返す
                    if (b != 0x03)
                        continue;

                    // バイト配列取得
                    byte[] buf = new byte[count - 1];
                    Array.Copy(this.ReadBuffer, byteIndex + 1, buf, 0, buf.Length - 1);

                    // 文字列取得
                    mes = GetString(buf, 0, buf.Length - 1);

                    // 取得した文字数取得
                    byteIndex += count + 1;
                    // カウント初期化
                    count = 0;

                    Console.WriteLine(mes);
                    File.AppendAllText("C:/Users/sasaki.TAROT/Desktop/log.txt", mes);

                }

                // 取得したバッファを保持バッファより削除
                Array.Copy(this.ReadBuffer, byteIndex, this.ReadBuffer, 0, this.ReadBuffer.Length - byteIndex);
                this.ReadBufferLength -= byteIndex;
            }
            catch (Exception err)
            {
                try
                {
                    // 文字列取得
                    mes = GetString(this.ReadBuffer, 0, byteIndex);
                }
                catch
                {
                }

                Console.WriteLine(mes);
                File.AppendAllText("C:/Users/sasaki.TAROT/Desktop/log.txt", mes + ":" + err);

                return;
            }

        }

        private string GetString(byte[] buf, int start, int length)
        {
            return Encoding.GetEncoding("Shift_JIS").GetString(buf, start, length);
        }

    }
}
