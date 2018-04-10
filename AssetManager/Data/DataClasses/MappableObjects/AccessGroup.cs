using System.Data;

namespace AssetManager.Data.Classes
{
    public class AccessGroup : MappableObject
    {
        #region Constructors

        public AccessGroup()
        {
        }

        public AccessGroup(DataRow data) : base(data)
        {
        }

        #endregion Constructors

        #region Properties

        [DataColumnName(SecurityCols.SecModule)]
        public string Name { get; set; }

        [DataColumnName(SecurityCols.AvailOffline)]
        public bool AvailableOffline { get; set; }

        [DataColumnName(SecurityCols.Description)]
        public string Description { get; set; }

        public override string Guid { get; set; }

        [DataColumnName(SecurityCols.AccessLevel)]
        public int Level { get; set; }

        public override string TableName { get; set; } = SecurityCols.TableName;

        #endregion Properties
    }
}
