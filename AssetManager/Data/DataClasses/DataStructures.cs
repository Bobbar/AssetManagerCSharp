using System.Drawing;
namespace AssetManager.Data.Classes
{
    public struct CodeAttribute
    {
        public string DisplayValue { get; set; }
        public string Code { get; set; }
        public int ID { get; set; }
        public Color Color { get; set; }
        
        public CodeAttribute(string displayValue, string code, int id)
        {
            this.DisplayValue = displayValue;
            this.Code = code;
            this.ID = id;
            this.Color = Color.Empty;
        }

        public CodeAttribute(string displayValue, string code, int id, Color color)
        {
            this.DisplayValue = displayValue;
            this.Code = code;
            this.ID = id;
            this.Color = color;
        }
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
