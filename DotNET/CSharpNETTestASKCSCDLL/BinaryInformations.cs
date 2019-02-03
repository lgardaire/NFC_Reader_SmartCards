using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpNETTestASKCSCDLL
{
    class BinaryInformations
    {
        private string binaryInfos;

        public BinaryInformations(byte infos)
        {
            binaryInfos = Convert.ToString(infos, 2).PadLeft(8, '0');
        }

        public int getLengthOfPayloadLength()
        {
            if (binaryInfos[3] == '1') // short record
            {
                return 1;
            }
            else
            {
                return 4;
            }
        }

        public bool isIdLengthPresent()
        {
            return binaryInfos[4] == '1';
        }

        public bool isWellKnownType()
        {
            return binaryInfos[5] == '0' && binaryInfos[6] == '0' && binaryInfos[7] == '1';
        }
    }
}
