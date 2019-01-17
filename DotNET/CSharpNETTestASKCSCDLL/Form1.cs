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
                                int maxLe = Convert.ToInt32(new Byte[]{maxLe1, maxLe2 });
                                int maxSize = Convert.ToInt32(new Byte[] { maxSize1, maxSize2 });
                                
                                iLenOut = 300;
                                byBuffIn = new byte[] { 0x00, 0xB0, 0x00, 0x00,  };
                                Status = AskReaderLib.CSC.CSC_ISOCommand(byBuffIn, byBuffIn.Length, byBuffOut, ref iLenOut);
                                if ((Status == AskReaderLib.CSC.RCSC_Ok) && (iLenOut > 2) && (byBuffOut[iLenOut - 2] == 0x90) && (byBuffOut[iLenOut - 1] == 0x00))
                                {
                                    Console.Write(byBuffOut);
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
    }
}