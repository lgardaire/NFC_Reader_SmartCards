using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpNETTestASKCSCDLL
{
    class MessageContent
    {
        public long type; //text type 0x54, URI type 0x55
        public String payload;
        public long lastIndex;
        public String language = "";

        public MessageContent(List<byte> payload, long type, long lastIndex)
        {
            this.type = type;
            if (type == 0x55) //URI type
            {
                this.payload = parseURIPayload(payload);
            } else if (type == 0x54) //text type 
            {
                Tuple<String, String> tmp = parseTextPayload(payload);
                this.payload = tmp.Item1;
                this.language = tmp.Item2;
            }
            
            this.lastIndex = lastIndex;
        }

        private String parseURIPayload(List<Byte> payload)
        {
            Dictionary<Byte, String> firstByteValue = setDictionaryFirstByte();
            return firstByteValue[payload[0]] + Encoding.ASCII.GetString(payload.GetRange(1, payload.Count - 1).ToArray());
        }

        private Tuple<String, String> parseTextPayload(List<Byte> payload)
        {
            String result = "";
            String infos = Convert.ToString(payload[0], 2).PadLeft(8, '0');
            int IANALanguageLength = Convert.ToInt32(infos.Substring(2), 2);
            int startIndex = IANALanguageLength + 1;
            String language = Encoding.ASCII.GetString(payload.GetRange(1, IANALanguageLength).ToArray());
            if (checkUTFValue(infos[0]) == 8)
            {
                result += Encoding.ASCII.GetString(payload.GetRange(startIndex, payload.Count - startIndex).ToArray());
            }
            else
            {
                result += Encoding.Unicode.GetString(payload.GetRange(startIndex, payload.Count - startIndex).ToArray());
            }
            return new Tuple<String, String>(result, language);
        }

        private int checkUTFValue(Char bit)
        {
            return bit == '0' ? 8 : 16;
        }

        private Dictionary<Byte, String> setDictionaryFirstByte()
        {
            Dictionary<Byte, String> firstByteValue = new Dictionary<byte, string>();
            firstByteValue[0x00] = "";
            firstByteValue[0x01] = "http://www.";
            firstByteValue[0x02] = "https://www.";
            firstByteValue[0x03] = "http://";
            firstByteValue[0x04] = "https://";
            firstByteValue[0x05] = "tel:";
            firstByteValue[0x06] = "mailto:";
            firstByteValue[0x07] = "ftp://anonymous:anonymous@";
            firstByteValue[0x08] = "ftp://ftp.";
            firstByteValue[0x09] = "ftps://";
            firstByteValue[0x0A] = "sftp://";
            firstByteValue[0x0B] = "smb://";
            firstByteValue[0x0C] = "nfs://";
            firstByteValue[0x0D] = "ftp://";
            firstByteValue[0x0E] = "dav://";
            firstByteValue[0x0F] = "news:";
            firstByteValue[0x10] = "telnet://";
            firstByteValue[0x11] = "imap:";
            firstByteValue[0x12] = "rtsp://";
            firstByteValue[0x13] = "urn:";
            firstByteValue[0x14] = "pop:";
            firstByteValue[0x15] = "sip:";
            firstByteValue[0x16] = "sips:";
            firstByteValue[0x17] = "tftp:";
            firstByteValue[0x18] = "btspp://";
            firstByteValue[0x19] = "btl2cap://";
            firstByteValue[0x1A] = "btgoep://";
            firstByteValue[0x1B] = "tcpobex://";
            firstByteValue[0x1C] = "irdaobex://";
            firstByteValue[0x1D] = "file://";
            firstByteValue[0x1E] = "urn:epc:id:";
            firstByteValue[0x1F] = "urn:epc:tag:";
            firstByteValue[0x20] = "urn:epc:pat:";
            firstByteValue[0x21] = "urn:epc:raw:";
            firstByteValue[0x22] = "urn:epc:";
            firstByteValue[0x23] = "urn:nfc:";
            return firstByteValue;
        }

        private Dictionary<Byte, String> setDictionaryLanguage()
        {
            Dictionary<Byte, String> firstByteValue = new Dictionary<byte, string>();
            firstByteValue[0x00] = "";
            firstByteValue[0x01] = "http://www.";
            firstByteValue[0x02] = "https://www.";
            firstByteValue[0x03] = "http://";
            firstByteValue[0x04] = "https://";
            firstByteValue[0x05] = "tel:";
            firstByteValue[0x06] = "mailto:";
            firstByteValue[0x07] = "ftp://anonymous:anonymous@";
            firstByteValue[0x08] = "ftp://ftp.";
            firstByteValue[0x09] = "ftps://";
            firstByteValue[0x0A] = "sftp://";
            firstByteValue[0x0B] = "smb://";
            firstByteValue[0x0C] = "nfs://";
            firstByteValue[0x0D] = "ftp://";
            firstByteValue[0x0E] = "dav://";
            firstByteValue[0x0F] = "news:";
            firstByteValue[0x10] = "telnet://";
            firstByteValue[0x11] = "imap:";
            firstByteValue[0x12] = "rtsp://";
            firstByteValue[0x13] = "urn:";
            firstByteValue[0x14] = "pop:";
            firstByteValue[0x15] = "sip:";
            firstByteValue[0x16] = "sips:";
            firstByteValue[0x17] = "tftp:";
            firstByteValue[0x18] = "btspp://";
            firstByteValue[0x19] = "btl2cap://";
            firstByteValue[0x1A] = "btgoep://";
            firstByteValue[0x1B] = "tcpobex://";
            firstByteValue[0x1C] = "irdaobex://";
            firstByteValue[0x1D] = "file://";
            firstByteValue[0x1E] = "urn:epc:id:";
            firstByteValue[0x1F] = "urn:epc:tag:";
            firstByteValue[0x20] = "urn:epc:pat:";
            firstByteValue[0x21] = "urn:epc:raw:";
            firstByteValue[0x22] = "urn:epc:";
            firstByteValue[0x23] = "urn:nfc:";
            return firstByteValue;
        }
    }
}
