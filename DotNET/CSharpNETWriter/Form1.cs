using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CSharpNETWriter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            AskReaderLib.CSC.sCARD_SearchExtTag SearchExtender;
            int Status;
            byte[] ATR;
            ATR = new byte[200];
            int lgATR;
            lgATR = 200;
            int Com=0;
            int SearchMask;

            txtCard.Text = "";

            try
            {
                AskReaderLib.CSC.SearchCSC();
                // user can also use line below to speed up coupler connection
                //AskReaderLib.CSC.Open ("COM2");

                // Define type of card to be detected: number of occurence for each loop
                SearchExtender.CONT = 0;
                SearchExtender.ISOB = 2;
                SearchExtender.ISOA = 2;
                SearchExtender.TICK = 1;
                SearchExtender.INNO = 2;
                SearchExtender.MIFARE = 0;
                SearchExtender.MV4k = 0;
                SearchExtender.MV5k = 0;
                SearchExtender.MONO = 0;

                Status = AskReaderLib.CSC.CSC_EHP_PARAMS_EXT(1, 1, 0, 0, 0, 0, 0, 0, null, 0, 0);
                // Define type of card to be detected
                SearchMask = AskReaderLib.CSC.SEARCH_MASK_ISOB | AskReaderLib.CSC.SEARCH_MASK_ISOA;
                Status = AskReaderLib.CSC.SearchCardExt(ref SearchExtender, SearchMask, 1, 20, ref Com, ref lgATR, ATR);

                if (Status != AskReaderLib.CSC.RCSC_Ok)
                    txtCard.Text =  "Error :" + Status.ToString ("X");

                if (Com == 2 | Com == 4 | Com == 8 | Com == 9 | Com == 12)
                {
                    if (Com == 2)
                        txtCard.Text = "ISO14443A-4 no Calypso";
                    else if (Com == 4)
                        txtCard.Text = "ISOB14443B-4 Calypso";
                    else if (Com == 8)
                        txtCard.Text = "ISO14443A-3 ";
                    else if (Com == 9)
                        txtCard.Text = "ISOB14443B-4 Calypso";
                    else if (Com == 12)
                        txtCard.Text = "ISO14443A-4 Calypso";

                    // select Application D2760000850101
                    Byte[] byBuffIn = new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x07, 0xD2, 0x76, 0x00, 0x00, 0x85, 0x01, 0x01, 0x00 };
                    int iLenOut = 300;
                    Byte[] byBuffOut = new byte[iLenOut];
                    Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                    if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                    {
                        iLenOut = 300;
                        // select EF CC File
                        byBuffIn = new byte[] { 0x00, 0xA4, 0x00, 0x0C, 0x02, 0xE1, 0x03 };
                        Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                        if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                        {
                            iLenOut = 300;
                            // Read binary CC File
                            byBuffIn = new byte[] { 0x00, 0xB0, 0x00, 0x00, 0x0F };
                            Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                            Byte maxLe1 = byBuffOut[4];
                            Byte maxLe2 = byBuffOut[5];
                            Byte maxLc1 = byBuffOut[6];
                            Byte maxLc2 = byBuffOut[7];
                            Byte lid1 = byBuffOut[10];
                            Byte lid2 = byBuffOut[11];
                            Byte maxSize1 = byBuffOut[12];
                            Byte maxSize2 = byBuffOut[13];

                            iLenOut = 300;
                            // select EF NDEF
                            byBuffIn = new byte[] { 0x00, 0xA4, 0x00, 0x0C, 0x02, lid1, lid2 };
                            Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                            if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                            {
                                byte[] data = transformData(comboBoxType.Text, txtCom.Text);

                                iLenOut = 3;
                                // write Binary NDEF
                                List<byte> byBuffInList = new List<byte>{};
                                byBuffInList.Add(0x00);
                                byBuffInList.Add(0xD6);
                                byBuffInList.Add(0x00);
                                byBuffInList.Add(0x00);
                                byBuffInList.Add(maxLc2);
                                byBuffInList.AddRange(data);
                                Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffInList.ToArray(), byBuffInList.Count, byBuffOut, ref iLenOut);
                                if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2))
                                {
                                    Console.Write("End of writing");
                                }
                            }
                        }
                    }
                } else if (Com == 0x6F)
                    txtCard.Text = "Card not found";
                else
                    txtCard.Text = "Card not found";
            }
            catch
            {
                MessageBox.Show("Error on trying do deal with reader");
            }
            AskReaderLib.CSC.Close();
        }

        private byte[] transformData(String type, String content)
        {
            List<byte> result = new List<byte>{ };
            List<byte> payload;
            if (type == "Text")
            {
                payload = createTextPayload(content, "fr");
                result.Add(createFirstByte(payload.Count));
                result.Add(0x01); //type length
                byte[] payloadLengthBytes = BitConverter.GetBytes(payload.Count);
                result.Add(payloadLengthBytes[0]); //payload length
                result.Add(0x54); //type
                result.AddRange(payload); //payload
            } else if (type == "URI")
            {
                String prefix = "http://";
                payload = createURIPayload(prefix, "www.polytech.fr");
                if(payload.Count == 0)
                {
                    throw new PrefixURINotFoundException("Prefix URI "+ prefix +" not found");
                }
                result.Add(createFirstByte(payload.Count));
                result.Add(0x01); //type length
                byte[] payloadLengthBytes = BitConverter.GetBytes(payload.Count);
                result.Add(payloadLengthBytes[0]); //payload length
                result.Add(0x55); //type
                result.AddRange(payload); //payload
            } else if (type == "SP")
            {
                result.Add(0x02); //type length
                result.Add(0x53);
                result.Add(0x70);
            }
            return result.ToArray();
        }

        private byte createFirstByte(int payloadLength)
        {
            String firstByte = "110";
            if (payloadLength <= 255)
            {
                firstByte += "1";
            } else
            {
                firstByte += "0";
            }
            firstByte += "0001";
            return Convert.ToByte(firstByte, 2);
        }

        private List<byte> createTextPayload(String content, String language)
        {
            List<byte> payloadBytes = new List<byte> { };
            payloadBytes.Add(0x02);
            byte[] bytes;
            if (language == "en")
            {
                bytes = Encoding.UTF8.GetBytes("en");
            } else //fr
            {
                bytes = Encoding.UTF8.GetBytes("fr");
            }
            payloadBytes.AddRange(bytes);
            payloadBytes.AddRange(Encoding.UTF8.GetBytes(content));
            return payloadBytes;
        }

        private List<byte> createURIPayload(String prefix, String restURI)
        {
            List<byte> payloadBytes = new List<byte> { };
            Dictionary<String, byte> dict = setDictionaryPrefix();
            byte value;
            if (dict.TryGetValue(prefix, out value))
            {
                payloadBytes.Add(value);
                payloadBytes.AddRange(Encoding.UTF8.GetBytes(restURI));
            }
            else
            {
                txtCard.Text = "Error : Prefix for URI not found";
                return new List<byte> { };
            }
            return payloadBytes;
        }

        private Dictionary<String, Byte> setDictionaryPrefix()
        {
            Dictionary<String, Byte> prefixValue = new Dictionary<string, byte>();
            prefixValue[""] = 0x00;
            prefixValue["http://www."] = 0x01;
            prefixValue["https://www."] = 0x02;
            prefixValue["http://"] = 0x03;
            prefixValue["https://"] = 0x04;
            prefixValue["tel:"] = 0x05;
            prefixValue["mailto:"] = 0x06;
            prefixValue["ftp://anonymous:anonymous@"] = 0x07;
            prefixValue["ftp://ftp."] = 0x08;
            prefixValue["ftps://"] = 0x09;
            prefixValue["sftp://"] = 0x0A;
            prefixValue["smb://"] = 0x0B;
            prefixValue["nfs://"] = 0x0C;
            prefixValue["ftp://"] = 0x0D;
            prefixValue["dav://"] = 0x0E;
            prefixValue["news:"] = 0x0F;
            prefixValue["telnet://"] = 0x10;
            prefixValue["imap:"] = 0x11;
            prefixValue["rtsp://"] = 0x12;
            prefixValue["urn:"] = 0x13;
            prefixValue["pop:"] = 0x14;
            prefixValue["sip:"] = 0x15;
            prefixValue["sips:"] = 0x16;
            prefixValue["tftp:"] = 0x17;
            prefixValue["btspp://"] = 0x18;
            prefixValue["btl2cap://"] = 0x19;
            prefixValue["btgoep://"] = 0x1A;
            prefixValue["tcpobex://"] = 0x1B;
            prefixValue["irdaobex://"] = 0x1C;
            prefixValue["file://"] = 0x1D;
            prefixValue["urn:epc:id:"] = 0x1E;
            prefixValue["urn:epc:tag:"] = 0x1F;
            prefixValue["urn:epc:pat:"] = 0x20;
            prefixValue["urn:epc:raw:"] = 0x21;
            prefixValue["urn:epc:"] = 0x22;
            prefixValue["urn:nfc:"] = 0x23;
            return prefixValue;
        }
    }
}