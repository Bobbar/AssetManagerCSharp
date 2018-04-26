using System.Collections.Generic;
using System.Drawing;

namespace AssetManager.Data.Classes
{
    /// <summary>
    /// Wrapper for storing and accessing a collection of <see cref="DbAttribute"/>.
    /// </summary>
    public class DbAttributes
    {
        private Dictionary<string, DbAttribute> attributes;

        /// <summary>
        /// Creates a new instance of the <see cref="DbAttributes"/> class.
        /// </summary>
        public DbAttributes()
        {
            attributes = new Dictionary<string, DbAttribute>();
        }

        /// <summary>
        /// Gets the <see cref="DbAttribute"/> from the collection with the specified code value.
        /// </summary>
        /// <param name="codeValue">The <see cref="DbAttribute.Code"/> of the attribute to return.</param>
        /// <returns>The <see cref="DbAttribute"/> in the collection with the specified <see cref="DbAttribute.Code"/>; otherwise will throw <see cref="KeyNotFoundException"/> if the <see cref="DbAttribute.Code"/> is not found.</returns>
        public DbAttribute this[string codeValue]
        {
            get
            {
                return attributes[codeValue];
            }
        }

        /// <summary>
        /// Creates and adds a new <see cref="DbAttribute"/> to the collection.
        /// </summary>
        /// <param name="displayValue">The human friendly display value for the attribute.</param>
        /// <param name="code">The non-friendly value stored in the database.</param>
        /// <param name="id">The unique database index ID for the attribute.</param>
        public void Add(string displayValue, string code, int id)
        {
            Add(displayValue, code, id, Color.Empty);
        }

        /// <summary>
        /// Creates and adds a new <see cref="DbAttribute"/> to the collection.
        /// </summary>
        /// <param name="displayValue">The human friendly display value for the attribute.</param>
        /// <param name="code">The non-friendly value stored in the database.</param>
        /// <param name="id">The unique database index ID for the attribute.</param>
        /// <param name="color">The hex color value associated with this attribute.</param>
        public void Add(string displayValue, string code, int id, Color color)
        {
            Add(new DbAttribute(displayValue, code, id, color));
        }

        /// <summary>
        /// Creates and adds a new <see cref="DbAttribute"/> to the collection.
        /// </summary>
        /// <param name="attribute">The new <see cref="DbAttribute"/> object to be added to the collection.</param>
        public void Add(DbAttribute attribute)
        {
            if (!attributes.ContainsKey(attribute.Code))
            {
                attributes.Add(attribute.Code, attribute);
            }
        }

        /// <summary>
        /// Returns an array of <see cref="DbAttribute"/> from the collection.
        /// </summary>
        /// <returns></returns>
        public DbAttribute[] GetArray()
        {
            var tmpList = new List<DbAttribute>();

            foreach (var attrib in attributes)
            {
                tmpList.Add(attrib.Value);
            }

            return tmpList.ToArray();
        }
    }

    public struct DbAttribute
    {
        private string displayValue;
        private string code;
        private int id;
        private Color color;

        public string DisplayValue { get { return displayValue; } }
        public string Code { get { return code; } }
        public int Id { get { return id; } }
        public Color Color { get { return color; } }

        public DbAttribute(string displayValue, string code, int id) : this(displayValue, code, id, Color.Empty)
        {
        }

        public DbAttribute(string displayValue, string code, int id, Color color)
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

    public static class Attributes
    {
        public static class SibiAttributes
        {
            public static DbAttributes StatusType;
            public static DbAttributes ItemStatusType;
            public static DbAttributes RequestType;
        }

        public static class DeviceAttributes
        {
            public static DbAttributes Locations;
            public static DbAttributes ChangeType;
            public static DbAttributes EquipType;
            public static DbAttributes OSType;
            public static DbAttributes StatusType;
        }
    }

    public struct DeviceUpdateInfo
    {
        public string Note;
        public string ChangeType;
    }

    public class LocalUser
    {
        public string UserName { get; }
        public string Fullname { get; }
        public int AccessLevel { get; }
        public string Guid { get; }

        public LocalUser() : this(string.Empty, string.Empty, 0, string.Empty)
        {
        }

        public LocalUser(string userName, string fullName, int accessLevel, string guid)
        {
            UserName = userName;
            Fullname = fullName;
            AccessLevel = accessLevel;
            Guid = guid;
        }
    }

    public class MunisEmployee
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }

        public MunisEmployee() : this(string.Empty, string.Empty)
        {
        }

        public MunisEmployee(string name, string number)
        {
            this.Name = name;
            this.Number = number;
            this.Guid = string.Empty;
        }

        public override string ToString()
        {
            string infoString = "{ Name: " + this.Name + " }  { Number: " + this.Number + "}";
            return infoString;
        }
    }

    public struct SmartEmpSearchInfo
    {
        public MunisEmployee SearchResult { get; set; }
        public string SearchString { get; set; }
        public int MatchDistance { get; set; }
        public int MatchLength { get; set; }

        public SmartEmpSearchInfo(MunisEmployee munisInfo, string searchValue) : this(munisInfo, searchValue, 0)
        {
        }

        public SmartEmpSearchInfo(MunisEmployee munisInfo, string searchValue, int matchDistance)
        {
            this.SearchResult = munisInfo;
            this.SearchString = searchValue;
            this.MatchLength = searchValue.Length;
            this.MatchDistance = matchDistance;
        }
    }
}