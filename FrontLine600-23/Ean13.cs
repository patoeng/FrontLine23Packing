
using System;

namespace FrontLine600_23
{
    public static class Ean13
    {
        public static string CalculateChecksumDigit(string twelveCode)
        {
            string sTemp = twelveCode;
            int iSum = 0;

            // Calculate the checksum digit here.
            for (int i = sTemp.Length; i >= 1; i--)
            {
                var iDigit = Convert.ToInt32(sTemp.Substring(i - 1, 1));
                if (i % 2 == 0)
                {
                    // odd
                    iSum += iDigit * 3;
                }
                else
                {
                    // even
                    iSum += iDigit * 1;
                }
            }

            int iCheckSum = (10 - (iSum % 10)) % 10;
            return sTemp + iCheckSum;

        }
    }
}
