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
        List<Tuple<String[], String>> recordsToWrite = new List<Tuple<String[], String>>();

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
            textBox1.Text = "";

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

                if (Com == 2 | Com == 4 | Com == 8 | Com == 9 | Com == 12)
                {
                    if(Com == 2)
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
                                int maxLe = Convert.ToInt32(maxLe1.ToString() + maxLe2.ToString());
                                int maxSize = Convert.ToInt32(maxSize1.ToString() + maxSize2.ToString());

                                iLenOut = 3;
                                // read Binary NDEF
                                byBuffIn = new byte[] { 0x00, 0xB0, 0x00, 0x00, 0x03 };
                                Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                                if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2))
                                {
                                    int maxSizeToRead = Convert.ToInt32(byBuffOut[1].ToString() + byBuffOut[2].ToString()) + 2;

                                    List<Byte> result = new List<Byte>();
                                    if (maxSizeToRead < maxLe) //need one iteration to read all data
                                    {
                                        iLenOut = maxLe;
                                        Byte[] tmp = BitConverter.GetBytes(maxSizeToRead);
                                        byBuffIn = new byte[] { 0x00, 0xB0, 0x00, 0x00, tmp[0] };
                                        Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                                        if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                                        {
                                            result.AddRange(byBuffOut);
                                        }
                                    } else //need multiple iterations
                                    {
                                    
                                        int iterations = (maxSizeToRead / maxLe) + 1;
                                        for(int i = 0; i < iterations; i++)
                                        {
                                            int size = maxLe;
                                            Byte[] offset;
                                            if (i == iterations - 1 && iterations > 1) //no need to read maxLe size in last iteration
                                            {
                                                size = maxSizeToRead - (maxLe * i) + 1;
                                            }
                                            //set the offset
                                            if(i == 0)
                                            {
                                                offset = BitConverter.GetBytes(0);
                                            } else
                                            {
                                                offset = BitConverter.GetBytes(i * maxLe -1);
                                            }
                                            byBuffIn = new byte[] { 0x00, 0xB0, offset[1], offset[0], maxLe2 };
                                            //read on card
                                            Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                                            if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                                            {
                                                byte[] array;
                                                if (i == 0)
                                                {
                                                    array = new byte[size];
                                                    Array.Copy(byBuffOut, array, size);
                                                } else
                                                {
                                                    array = new byte[size];
                                                    for (int j = 1; j <= size; j++)
                                                    {
                                                        array[j-1] = byBuffOut[j];
                                                    }
                                                }
                                                result.AddRange(array);
                                            }
                                        } 
                                    }

                                    long startIndex = 3;
                                    int totalLength = result.Count;
                                    List<MessageContent> messages = new List<MessageContent>();
                                    // iterations to analyse all records, startIndex is index to know where is the start of the current record
                                    while (startIndex < totalLength)
                                    {
                                        Tuple<List<MessageContent>, bool> temporaryResult = analyseData(result, startIndex);
                                        messages.AddRange(temporaryResult.Item1);
                                        if (!temporaryResult.Item2) { 
                                            startIndex = temporaryResult.Item1[temporaryResult.Item1.Count - 1].lastIndex;
                                        } else
                                        {
                                            break;
                                        }
                                    }
                                    //loop to display the records in the UI 
                                    for(int i = 0; i < messages.Count; i++)
                                    {
                                        //if it is a Text record
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
                                else
                                {
                                    MessageBox.Show("Can't read on card");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Can't select EF NDEF");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Can't select CC File");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Can't select application");
                    }
                }
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

        //Function to analyse the records payloads and transform them
        private Tuple<List<MessageContent>, bool> analyseData(List<Byte> content, long startIndex)
        {
            try { 
                //Analyse tehe first byte containing useful informations
                BinaryInformations binaryInfos = new BinaryInformations(content.ToArray()[startIndex]);
                if (binaryInfos.isWellKnownType())
                {
                    //Get all needed data in the payload
                    int payLoadLengthLength = binaryInfos.getLengthOfPayloadLength();
                    bool idLengthPresent = binaryInfos.isIdLengthPresent();

                    long payloadLength = convertByte(getSubListFrom(content.ToArray(), startIndex + 2, payLoadLengthLength));
                    long typeLength = convertByte(content.ToArray()[startIndex + 1]);
                    int idLengthLength = idLengthPresent ? 1 : 0;
                
                    //analyse the type of record
                    long type = -1;
                    if (typeLength != 0)
                    {
                        type = convertSublistByteToLong(content, startIndex + payLoadLengthLength + idLengthLength + 2, typeLength);
                    } else
                    {
                        type = 0;
                    }

                    //find the Id length
                    long idLength = 0;
                    if (idLengthLength != 0)
                    {
                        idLength = convertSublistByteToLong(content, startIndex + payLoadLengthLength + 1, idLengthLength);
                    }

                    //find the start index in the record where the data is
                    long payloadStartIndex;
                    if (type != 0)
                    {
                        payloadStartIndex = startIndex + payLoadLengthLength + idLengthLength + typeLength + idLength + 2;
                    } else
                    {
                        payloadStartIndex = startIndex + payLoadLengthLength + 2;
                    }
                    List<Byte> payload = getSubListFrom(content.ToArray(), payloadStartIndex, Math.Min(payloadLength, content.Count-payloadStartIndex));

                    if (type == 0x5370) // if it is a SmartPoster
                    {
                        //store the last index of each record in the SmartPoster to separate them
                        List<MessageContent> result = new List<MessageContent>();
                        long lastIndex = 0;
                        while(lastIndex < payloadLength)
                        {
                            // analyse each record
                            Tuple<List<MessageContent>, bool> messContent = analyseData(payload, lastIndex);
                            result.Add(messContent.Item1[0]);
                            lastIndex = messContent.Item1[0].lastIndex;
                        }
                        return new Tuple<List<MessageContent>, bool>(result, binaryInfos.isLast());
                    } else //if it is not a SmartPoster
                    {
                        //just analyse the record
                        List<MessageContent> messageResult = new List<MessageContent>();
                        messageResult.Add(new MessageContent(payload, type, payloadStartIndex+payloadLength));
                        return new Tuple<List<MessageContent>, bool>(messageResult, binaryInfos.isLast());
                    }
                } else
                {
                    return new Tuple<List<MessageContent>, bool>(new List<MessageContent>(), true);
                } 
            } catch {
                return new Tuple<List<MessageContent>, bool>(new List<MessageContent>(), false);
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

        //Function to write on the card
        private void button2_Click(object sender, EventArgs e)
        {
            AskReaderLib.CSC.sCARD_SearchExtTag SearchExtender;
            int Status;
            byte[] ATR;
            ATR = new byte[200];
            int lgATR;
            lgATR = 200;
            int Com = 0;
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
                    txtCard.Text = "Error :" + Status.ToString("X");

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
                                //prepare the payload to write
                                //recordsToWrite contains the data the user input by the UI
                                List<byte> payload = new List<byte>();
                                for(int i = 0; i < recordsToWrite.Count; i++)
                                {
                                    //handle messageEnd and messageBegin
                                    bool first = false;
                                    if (i == 0) first = true;
                                    bool last = false;
                                    if (i == recordsToWrite.Count - 1) last = true;

                                    //create record depending on the type
                                    string type = recordsToWrite[i].Item2;
                                    if(type == "t")
                                    {
                                        payload.AddRange(transformDataForText(recordsToWrite[i].Item1[0], first, last));
                                    } else if (type == "u")
                                    {
                                        payload.AddRange(transformDataForURI(recordsToWrite[i].Item1[1], recordsToWrite[i].Item1[0], first, last));
                                    } else if (type == "r")
                                    {
                                        payload.AddRange(transformDataForBinary(recordsToWrite[i].Item1[0], first, last));
                                    }
                                }
                                textBox3.Text = "";
                                recordsToWrite = new List<Tuple<string[], string>>();

                                iLenOut = 300;

                                // write Binary NDEF
                                int Lc = Convert.ToInt32(maxLc1.ToString() + maxLc2.ToString());
                                Byte[] dataLengthBytes = BitConverter.GetBytes(payload.Count);
                                Byte[] payloadLengthBytes = BitConverter.GetBytes(payload.Count + 2);

                                payload.Insert(0, payloadLengthBytes[0]);
                                payload.Insert(1, dataLengthBytes[1]);
                                payload.Insert(2, dataLengthBytes[0]);
                                List<byte> byBuffInList = new List<byte> { 0x00, 0xD6, 0x00, 0x00 };
                                byBuffInList.AddRange(payload);
                                Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffInList.ToArray(), byBuffInList.Count, byBuffOut, ref iLenOut);
                                if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2))
                                {
                                    Console.Write("End of writing");
                                }
                                else
                                {
                                    MessageBox.Show("Can't write on card");
                                }
                            }
                        }
                    }
                }
                else if (Com == 0x6F)
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

        private byte[] transformDataForText(String content, bool firstRecord, bool lastRecord)
        {
            List<byte> result = new List<byte> { };
            List<byte> payload = createTextPayload(content, "fr");
            result.Add(createFirstByte(payload.Count, firstRecord, lastRecord));
            result.Add(0x01); //type length
            byte[] payloadLengthBytes = BitConverter.GetBytes(payload.Count);
            result.Add(payloadLengthBytes[0]); //payload length
            result.Add(0x54); //type
            result.AddRange(payload); //payload
            return result.ToArray();
        }

        private byte[] transformDataForURI(String prefix, String content, bool firstRecord, bool lastRecord)
        {
            List<byte> result = new List<byte> { };
            List<byte> payload = createURIPayload(prefix, content);
            if (payload.Count == 0)
            {
                throw new PrefixURINotFoundException("Prefix URI " + prefix + " not found");
            }
            result.Add(createFirstByte(payload.Count, firstRecord, lastRecord));
            result.Add(0x01); //type length
            byte[] payloadLengthBytes = BitConverter.GetBytes(payload.Count);
            result.Add(payloadLengthBytes[0]); //payload length
            result.Add(0x55); //type
            result.AddRange(payload); //payload
            return result.ToArray();
        }

        private byte[] transformDataForBinary(String content, bool firstRecord, bool lastRecord)
        {
            List<byte> result = new List<byte> { };
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(content);
            result.Add(createFirstByte(payload.Length, firstRecord, lastRecord));
            result.Add(0x00); //type length
            byte[] payloadLengthBytes = BitConverter.GetBytes(payload.Length);
            result.Add(payloadLengthBytes[0]); //payload length
            result.AddRange(payload); //payload
            return result.ToArray();
        }

        private byte createFirstByte(int payloadLength, bool firstRecord, bool lastRecord)
        {
            String firstByte = "";
            if (firstRecord)
            {
                firstByte += "1";
            } else
            {
                firstByte += "0";
            }
            if (lastRecord)
            {
                firstByte += "1";
            }
            else
            {
                firstByte += "0";
            }
            firstByte += "0";
            if (payloadLength <= 255)
            {
                firstByte += "1";
            }
            else
            {
                firstByte += "0";
            }
            firstByte += "0001";
            return Convert.ToByte(firstByte, 2);
        }

        //create record of Text type following the norm
        private List<byte> createTextPayload(String content, String language)
        {
            List<byte> payloadBytes = new List<byte> { };
            payloadBytes.Add(0x02);
            byte[] bytes;
            if (language == "en")
            {
                bytes = Encoding.UTF8.GetBytes("en");
            }
            else //fr
            {
                bytes = Encoding.UTF8.GetBytes("fr");
            }
            payloadBytes.AddRange(bytes);
            payloadBytes.AddRange(Encoding.UTF8.GetBytes(content));
            return payloadBytes;
        }

        //create record of URI type following the norm
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

        //dictionnary of each usable URI prefixes
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

        //Add button to add a record to the records to write
        private void button3_Click(object sender, EventArgs e)
        {
            String type = comboBoxType.Text;
            if (type == "Text")
            {
                
                recordsToWrite.Add(new Tuple<String[], String>(new String[] { textBoxContent.Text }, "t"));
                textBox3.AppendText("Text : "+ textBoxContent.Text);
                textBox3.AppendText(Environment.NewLine);
                textBoxContent.Text = "";
            }
            else if (type == "URI")
            {
                recordsToWrite.Add(new Tuple<String[], String>(new String[] { textBoxContent.Text, textBoxPrefix.Text }, "u"));
                textBox3.AppendText("URI : " + textBoxPrefix.Text + textBoxContent.Text);
                textBox3.AppendText(Environment.NewLine);
                textBoxContent.Text = "";
                textBoxPrefix.Text = "";
            }
            else if (type == "Raw")
            {
                recordsToWrite.Add(new Tuple<String[], String>(new String[] { textBoxContent.Text }, "r"));
                textBox3.AppendText("Raw : " + textBoxContent.Text);
                textBox3.AppendText(Environment.NewLine);
                textBoxContent.Text = "";
            }
        }
    }
}