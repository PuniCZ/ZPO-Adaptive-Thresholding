using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AdaptiveThresholding
{
    public class DeviceManager
    {
        [DllImport("avicap32.dll")]
        protected static extern bool capGetDriverDescriptionA(short wDriverIndex,
            [MarshalAs(UnmanagedType.VBByRefStr)]ref String lpszName,
            int cbName, [MarshalAs(UnmanagedType.VBByRefStr)] ref String lpszVer, int cbVer);

        static ArrayList devices = new ArrayList();

        public static CamDevice[] GetAllDevices()
        {
            String dName = "".PadRight(100);
            String dVersion = "".PadRight(100);

            for (short i = 0; i < 10; i++)
            {
                if (capGetDriverDescriptionA(i, ref dName, 100, ref dVersion, 100))
                {
                    CamDevice d = new CamDevice(i);
                    d.Name = dName.Trim();
                    d.Version = dVersion.Trim();

                    devices.Add(d);
                }
            }

            return (CamDevice[])devices.ToArray(typeof(CamDevice));
        }

        public static CamDevice GetDevice(int deviceIndex)
        {
            return (CamDevice)devices[deviceIndex];
        }
    }
}
