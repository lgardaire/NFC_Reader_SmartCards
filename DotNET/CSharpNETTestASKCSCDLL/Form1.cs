using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CSharpNETTestASKCSCDLL
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

            txtCom.Text = "";
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
                    txtCom.Text =  "Error :" + Status.ToString ("X");
                else
                    txtCom.Text = Com.ToString("X");

                if (Com == 2) { 
                    txtCard.Text = "ISO14443A-4 no Calypso";
                    Byte[] byBuffIn = new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x07, 0xD2, 0x76, 0x00, 0x00, 0x85, 0x01, 0x01, 0x00 };
                    int iLenOut = 300;
                    Byte[] byBuffOut = new byte[iLenOut];
                    Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                    if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                    {
                        iLenOut = 300;
                        byBuffIn = new byte[] { 0x00, 0xA4, 0x00, 0x0C, 0x02, 0xE1, 0x03 };
                        Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                        if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                        {
                            iLenOut = 300;
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
                            byBuffIn = new byte[] { 0x00, 0xA4, 0x00, 0x0C, 0x02, lid1, lid2 };
                            Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                            if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                            {
                                int maxLe = Convert.ToInt32(maxLe1.ToString() + maxLe2.ToString());
                                int maxSize = Convert.ToInt32(maxSize1.ToString() + maxSize2.ToString());

                                iLenOut = 3;
                                byBuffIn = new byte[] { 0x00, 0xB0, 0x00, 0x00, 0x03 };
                                Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                                if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2))
                                {

                                    int maxSizeToRead = Convert.ToInt32(byBuffOut[1].ToString() + byBuffOut[2].ToString());

                                    List<Byte> result = new List<Byte>();
                                    if (maxSizeToRead < maxLe)
                                    {
                                        iLenOut = maxLe;
                                        Byte[] tmp = BitConverter.GetBytes(maxSizeToRead+2);
                                        byBuffIn = new byte[] { 0x00, 0xB0, 0x00, 0x00, tmp[0] };
                                        Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                                        if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                                        {
                                            result.AddRange(byBuffOut);
                                        }
                                    } else
                                    {
                                    
                                        int iterations = (maxSizeToRead / maxLe) + 1;
                                        for(int i = 0; i < iterations; i++)
                                        {
                                            Byte[] offset = BitConverter.GetBytes(i * maxLe + 3);
                                            iLenOut = maxLe + 3;
                                            byBuffIn = new byte[] { 0x00, 0xB0, offset[0], offset[1], maxLe2 };
                                            Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                                            if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                                            {
                                                result.AddRange(byBuffOut);
                                            }
                                        } 
                                    }

                                    long startIndex = 3;
                                    int totalLength = result.Count;
                                    List<MessageContent> messages = new List<MessageContent>();
                                    while (startIndex < totalLength)
                                    {
                                        List<MessageContent> temporaryResult = analyseData(result, startIndex);
                                        messages.AddRange(temporaryResult);
                                        if (temporaryResult.Count != 0) { 
                                            startIndex = temporaryResult[temporaryResult.Count - 1].lastIndex;
                                        } else
                                        {
                                            break;
                                        }
                                    }
                                    textBox1.Clear();
                                    for(int i = 0; i < messages.Count; i++)
                                    {
                                        if(messages[i].language != "")
                                        {
                                            textBox1.AppendText(messages[i].payload + " (Language: " + messages[i].language + ")");
                                            textBox1.AppendText(Environment.NewLine);
                                        } else
                                        {
                                            textBox1.AppendText(messages[i].payload);
                                            textBox1.AppendText(Environment.NewLine);
                                        }
                                    }
                                    Console.Write("End of reading");

                                }
                            }
                        }
                    }
                }
                else if (Com == 4)
                    txtCard.Text = "ISOB14443B-4 Calypso";
                else if (Com == 8)
                    txtCard.Text = "ISO14443A-3 ";
                else if (Com == 9)
                    txtCard.Text = "ISOB14443B-4 Calypso";
                else if (Com == 12)
                    txtCard.Text = "ISO14443A-4 Calypso";
                else if (Com == 0x6F)
                    txtCard.Text = "Card not found";
                else
                    txtCard.Text = "";
            }
            catch
            {
                MessageBox.Show("Error on trying do deal with reader");
            }
            AskReaderLib.CSC.Close();
        }

        private bool checkId(string binary)
        {
            return binary[4] == '1';
        }

        private List<MessageContent> analyseData(List<Byte> content, long startIndex)
        {
            try { 
                BinaryInformations binaryInfos = new BinaryInformations(content.ToArray()[startIndex]);
                if (binaryInfos.isWellKnownType())
                {
                    int payLoadLengthLength = binaryInfos.getLengthOfPayloadLength();
                    bool idLengthPresent = binaryInfos.isIdLengthPresent();

                    long payloadLength = convertByte(getSubListFrom(content.ToArray(), startIndex + 2, payLoadLengthLength));
                    long typeLength = convertByte(content.ToArray()[startIndex + 1]);
                    int idLengthLength = idLengthPresent ? 1 : 0;
                
                    long type = -1;
                    if (typeLength != 0)
                    {
                        type = convertSublistByteToLong(content, startIndex + payLoadLengthLength + idLengthLength + 2, typeLength);
                    }

                    long idLength = 0;
                    if (idLengthLength != 0)
                    {
                        idLength = convertSublistByteToLong(content, startIndex + payLoadLengthLength + 1, idLengthLength);
                    }

                    long payloadStartIndex = startIndex + payLoadLengthLength + idLengthLength + typeLength + idLength + 2;
                    List<Byte> payload = getSubListFrom(content.ToArray(), payloadStartIndex, payloadLength);

                    if (type == 0x5370) //is SmartPoster
                    {
                        List<MessageContent> result = new List<MessageContent>();
                        long lastIndex = 0;
                        while(lastIndex < payloadLength)
                        {
                            List<MessageContent> messContent = analyseData(payload, lastIndex);
                            result.Add(messContent[0]);
                            lastIndex = messContent[0].lastIndex;
                        }
                        return result;
                    } else
                    {
                        List<MessageContent> messageResult = new List<MessageContent>();
                        messageResult.Add(new MessageContent(payload, type, payloadStartIndex+payloadLength));
                        return messageResult;
                    }
                } else
                {
                    return new List<MessageContent>();
                } 
            } catch {
                return new List<MessageContent>();
            }
        }

        private long convertSublistByteToLong(List<byte> list, long startIndex, long length)
        {
            List<Byte> sublist = getSubListFrom(list.ToArray(), startIndex, length);
            sublist.Reverse();
            return convertByte(sublist);
        }

        private long convertByte(List<Byte> list)
        {
            while (list.Count < 4)
            {
                list.Add(0x00);
            }
            return BitConverter.ToInt32(list.ToArray(), 0);
        }

        private long convertByte(Byte byte1)
        {
            List<Byte> tmp = new List<Byte>();
            tmp.Add(byte1);
            return convertByte(tmp);
        }

        private List<Byte> getSubListFrom(Byte[] list, long startIndex, long length)
        {
            List<Byte> sublist = new List<Byte>();
            for(long i = startIndex ; i < startIndex+length; i++)
            {
                sublist.Add(list[i]);
            }
            return sublist;
        }
    }
}