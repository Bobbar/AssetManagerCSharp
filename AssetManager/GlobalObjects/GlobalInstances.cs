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
            public AttributeDataStruct[] StatusType;
            public AttributeDataStruct[] ItemStatusType;
            public AttributeDataStruct[] RequestType;
        }

        public class DeviceAttributes
        {
            public AttributeDataStruct[] Locations;
            public AttributeDataStruct[] ChangeType;
            public AttributeDataStruct[] EquipType;
            public AttributeDataStruct[] OSType;
            public AttributeDataStruct[] StatusType;
        }

        public static DeviceAttributes DeviceAttribute = new DeviceAttributes();
        public static SibiAttributes SibiAttribute = new SibiAttributes();
        public static MunisFunctions MunisFunc = new MunisFunctions();
        //public static AssetManagerFunctions AssetFunc = new AssetManagerFunctions();

        public static FtpFunctions FTPFunc = new FtpFunctions();
    }
}