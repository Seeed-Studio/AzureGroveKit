using System;
using System.Threading.Tasks;

namespace ms_sample
{
    class Utility
    {
        public static async Task<String> getMacAddress()
        {
            //string strHostName = Dns.GetHostName();
            //IPHostEntry ipEntry = await Dns.GetHostEntryAsync(strHostName);
            //string strAddr = ipEntry.AddressList[0].ToString();
            //return strAddr;

            // get UUID with asscoate PI3 hardware
            await Task.Delay(100);
            return "testDevice001";
        }

    }
}
