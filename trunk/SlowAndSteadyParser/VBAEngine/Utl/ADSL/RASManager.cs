using System;
using System.Runtime.InteropServices;

namespace SlowAndSteadyParser
{
    public class RasManager
    {
        public const int RAS_MaxEntryName = 256;
        public const int RAS_MaxPhoneNumber = 128;
        public const int UNLEN = 256;
        public const int PWLEN = 256;
        public const int DNLEN = 15;
        public const int MAX_PATH = 260;
        public const int RAS_MaxDeviceType = 16;
        public const int RAS_MaxCallbackNumber = RAS_MaxPhoneNumber;

        public delegate void Callback(uint unMsg, int rasconnstate, int dwError);

        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
        public struct RASDIALPARAMS
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
            public string szEntryName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxPhoneNumber + 1)]
            public string szPhoneNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxCallbackNumber + 1)]
            public string szCallbackNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UNLEN + 1)]
            public string szUserName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PWLEN + 1)]
            public string szPassword;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DNLEN + 1)]
            public string szDomain;
            public int dwSubEntry;
            public int dwCallbackId;
        }

        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RasDial(int lpRasDialExtensions, string lpszPhonebook, ref   RASDIALPARAMS lprasdialparams, int dwNotifierType, Callback lpvNotifier, ref   int lphRasConn);

        private RASDIALPARAMS RasDialParams;
        private int Connection;

        public RasManager()
        {
            Connection = 0;
            RasDialParams = new RASDIALPARAMS();
            RasDialParams.dwSize = Marshal.SizeOf(RasDialParams);
        }

        #region   Properties
        public string UserName
        {
            get
            {
                return RasDialParams.szUserName;
            }
            set
            {
                RasDialParams.szUserName = value;
            }
        }

        public string Password
        {
            get
            {
                return RasDialParams.szPassword;
            }
            set
            {
                RasDialParams.szPassword = value;
            }
        }

        public string EntryName
        {
            get
            {
                return RasDialParams.szEntryName;
            }
            set
            {
                RasDialParams.szEntryName = value;
            }
        }
        #endregion

        public int Connect()
        {
            Callback rasDialFunc = new Callback(RasManager.RasDialFunc);
            GCHandle gch = GCHandle.Alloc(rasDialFunc);
            RasDialParams.szEntryName += "\0";
            RasDialParams.szUserName += "\0";
            RasDialParams.szPassword += "\0";
            int result = RasDial(0, null, ref   RasDialParams, 0, rasDialFunc, ref Connection);
            gch.Free();
            return result;
        }

        public static void RasDialFunc(uint unMsg, int rasconnstate, int dwError)
        {
        }
    }
}
