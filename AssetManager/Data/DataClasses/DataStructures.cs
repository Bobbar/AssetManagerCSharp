using System.Drawing;
namespace AssetManager.Data.Classes
{
    public struct DBCode
    {
        private string displayValue;
        private string code;
        private int id;
        private Color color;

        public string DisplayValue { get { return displayValue; } }
        public string Code { get { return code; } }
        public int Id { get { return id; } }
        public Color Color { get { return color; } }

        public DBCode(string displayValue, string code, int id)
        {
            this.displayValue = displayValue;
            this.code = code;
            this.id = id;
            this.color = Color.Empty;
        }

        public DBCode(string displayValue, string code, int id, Color color)
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
            public DBCode[] StatusType;
            public DBCode[] ItemStatusType;
            public DBCode[] RequestType;
        }

        public class DeviceAttributes
        {
            public DBCode[] Locations;
            public DBCode[] ChangeType;
            public DBCode[] EquipType;
            public DBCode[] OSType;
            public DBCode[] StatusType;
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
        public string Guid;

        public LocalUser(string userName, string fullName, int accessLevel, string guid)
        {
            UserName = userName;
            Fullname = fullName;
            AccessLevel = accessLevel;
            Guid = guid;
        }
    }

}
namespace AssetManager.Data.Classes
{

    public class MunisEmployee
    {
        public string Number;
        public string Name;
        public string Guid;

        public MunisEmployee(string name, string number)
        {
            this.Name = name;
            this.Number = number;
            this.Guid = string.Empty;
        }

        public MunisEmployee()
        {
            this.Name = string.Empty;
            this.Number = string.Empty;
            this.Guid = string.Empty;
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


        public SmartEmpSearchInfo(MunisEmployee munisInfo, string searchValue, int matchDistance)
        {
            this.SearchResult = munisInfo;
            this.SearchString = searchValue;
            MatchLength = searchValue.Length;
            this.MatchDistance = matchDistance;

        }

        public SmartEmpSearchInfo(MunisEmployee munisInfo, string searchValue)
        {
            this.SearchResult = munisInfo;
            this.SearchString = searchValue;
            MatchLength = searchValue.Length;
            this.MatchDistance = 0;

        }
    }
}
