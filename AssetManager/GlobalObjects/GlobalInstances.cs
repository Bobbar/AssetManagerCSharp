using System.Collections.Generic;
using AssetManager.Data.Classes;
using AssetManager.Data.Functions;

namespace AssetManager
{
    internal static class GlobalInstances
    {
        public static Dictionary<string, string> DepartmentCodes;
        public class SibiAttributes
        {
            public CodeAttribute[] StatusType;
            public CodeAttribute[] ItemStatusType;
            public CodeAttribute[] RequestType;
        }

        public class DeviceAttributes
        {
            public CodeAttribute[] Locations;
            public CodeAttribute[] ChangeType;
            public CodeAttribute[] EquipType;
            public CodeAttribute[] OSType;
            public CodeAttribute[] StatusType;
        }

        public static DeviceAttributes DeviceAttribute = new DeviceAttributes();
        public static SibiAttributes SibiAttribute = new SibiAttributes();
        public static FtpFunctions FTPFunc = new FtpFunctions();
    }
}