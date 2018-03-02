using System.Drawing;
namespace AssetManager.Data.Classes
{
    public struct CodeAttribute
    {
        private string displayValue;
        private string code;
        private int id;
        private Color color;

        public string DisplayValue { get { return displayValue; } }
        public string Code { get { return code; } }
        public int ID { get { return id; } }
        public Color Color { get { return color; } }

        public CodeAttribute(string displayValue, string code, int id)
        {
            this.displayValue = displayValue;
            this.code = code;
            this.id = id;
            this.color = Color.Empty;
        }

        public CodeAttribute(string displayValue, string code, int id, Color color)
        {
            this.displayValue = displayValue;
            this.code = code;
            this.id = id;
            this.color = color;
        }

        public override string ToString()
        {
            return DisplayValue;
        }
    }
}

namespace AssetManager.Data.Classes
{
    public static class Attributes
    {
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

    }
}

namespace AssetManager.Data.Classes
{

    public struct DeviceUpdateInfo
    {
        public string Note;
        public string ChangeType;
    }
}
namespace AssetManager.Data.Classes
{

    public struct LocalUser
    {
        public string UserName;
        public string Fullname;
        public int AccessLevel;
        public string GUID;

        public LocalUser(string userName, string fullName, int accessLevel, string guid)
        {
            UserName = userName;
            Fullname = fullName;
            AccessLevel = accessLevel;
            GUID = guid;
        }
    }

}
namespace AssetManager.Data.Classes
{

    public class MunisEmployee
    {
        public string Number;
        public string Name;
        public string GUID;

        public MunisEmployee(string name, string number)
        {
            this.Name = name;
            this.Number = number;
            this.GUID = string.Empty;
        }

        public MunisEmployee()
        {
            this.Name = string.Empty;
            this.Number = string.Empty;
            this.GUID = string.Empty;
        }

        public override string ToString()
        {
            string infoString = "{ Name: " + this.Name + " }  { Number: " + this.Number + "}";
            return infoString;
        }
    }
}
namespace AssetManager.Data.Classes
{

    public struct SmartEmpSearchInfo
    {
        public MunisEmployee SearchResult { get; set; }
        public string SearchString { get; set; }
        public int MatchDistance { get; set; }
        public int MatchLength { get; set; }


        public SmartEmpSearchInfo(MunisEmployee munisInfo, string searchString, int matchDistance)
        {
            this.SearchResult = munisInfo;
            this.SearchString = searchString;
            MatchLength = searchString.Length;
            this.MatchDistance = matchDistance;

        }

        public SmartEmpSearchInfo(MunisEmployee munisInfo, string searchString)
        {
            this.SearchResult = munisInfo;
            this.SearchString = searchString;
            MatchLength = searchString.Length;
            this.MatchDistance = 0;

        }
    }
}
