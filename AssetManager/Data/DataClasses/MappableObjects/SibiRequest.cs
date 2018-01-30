using System.Data;

namespace AssetManager.Data.Classes
{
    public class SibiRequest : MappableObject
    {
        #region Fields

        public DataTable RequestItems { get; set; }

        #endregion Fields

        #region Constructors

        public SibiRequest()
        {
            this.GUID = System.Guid.NewGuid().ToString();
        }

        public SibiRequest(DataTable data) : base(data)
        {
        }

        #endregion Constructors

        #region Properties

        [DataColumnName(SibiRequestCols.DateStamp)]
        public System.DateTime DateStamp { get; set; }

        [DataColumnName(SibiRequestCols.Description)]
        public string Description { get; set; }

        [DataColumnName(SibiRequestCols.UID)]
        public override string GUID { get; set; }

        [DataColumnName(SibiRequestCols.NeedBy)]
        public System.DateTime NeedByDate { get; set; }

        [DataColumnName(SibiRequestCols.PO)]
        public string PO { get; set; }

        [DataColumnName(SibiRequestCols.ReplaceAsset)]
        public string ReplaceAsset { get; set; }

        [DataColumnName(SibiRequestCols.ReplaceSerial)]
        public string ReplaceSerial { get; set; }

        [DataColumnName(SibiRequestCols.RequestNumber)]
        public string RequestNumber { get; set; }

        [DataColumnName(SibiRequestCols.Type)]
        public string RequestType { get; set; }

        [DataColumnName(SibiRequestCols.RequestUser)]
        public string RequestUser { get; set; }

        [DataColumnName(SibiRequestCols.RequisitionNumber)]
        public string RequisitionNumber { get; set; }

        [DataColumnName(SibiRequestCols.RTNumber)]
        public string RTNumber { get; set; }

        [DataColumnName(SibiRequestCols.Status)]
        public string Status { get; set; }

        public override string TableName { get; set; } = SibiRequestCols.TableName;

        #endregion Properties
    }
}
