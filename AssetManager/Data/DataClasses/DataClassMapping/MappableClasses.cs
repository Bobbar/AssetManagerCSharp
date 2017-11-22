using System.Data;

namespace AssetManager
{
    public class DeviceObject : DataMappingObject
    {
        #region Constructors

        public DeviceObject()
        {
        }

        public DeviceObject(object data) : base(data)
        {
        }

        public DeviceObject(string GUID)
        {
            this.MapClassProperties(GetDeviceDataFromGUID(GUID));
        }

        #endregion Constructors

        #region Properties

        [DataColumnName(DevicesCols.AssetTag)]
        public string AssetTag { get; set; }

        [DataColumnName(DevicesCols.CurrentUser)]
        public string CurrentUser { get; set; }

        [DataColumnName(DevicesCols.MunisEmpNum)]
        public string CurrentUserEmpNum { get; set; }

        [DataColumnName(DevicesCols.Description)]
        public string Description { get; set; }

        [DataColumnName(DevicesCols.EQType)]
        public string EquipmentType { get; set; }

        public string FiscalYear { get; set; }

        [DataColumnName(DevicesCols.DeviceUID)]
        public override string GUID { get; set; }

        public DeviceHistoricalObject Historical { get; set; } = new DeviceHistoricalObject();

        [DataColumnName(DevicesCols.HostName)]
        public string HostName { get; set; }

        [DataColumnName(DevicesCols.Trackable)]
        public bool IsTrackable { get; set; }

        [DataColumnName(DevicesCols.Location)]
        public string Location { get; set; }

        public string Note { get; set; }

        [DataColumnName(DevicesCols.OSVersion)]
        public string OSVersion { get; set; }

        [DataColumnName(DevicesCols.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DataColumnName(DevicesCols.PO)]
        public string PO { get; set; }

        [DataColumnName(DevicesCols.PurchaseDate)]
        public System.DateTime PurchaseDate { get; set; }

        [DataColumnName(DevicesCols.ReplacementYear)]
        public string ReplaceYear { get; set; }

        [DataColumnName(DevicesCols.Serial)]
        public string Serial { get; set; }

        [DataColumnName(DevicesCols.SibiLinkUID)]
        public string SibiLink { get; set; }

        [DataColumnName(DevicesCols.Status)]
        public string Status { get; set; }

        public override string TableName { get; set; } = DevicesCols.TableName;

        public DeviceTrackingObject Tracking { get; set; } = new DeviceTrackingObject();

        #endregion Properties

        #region Methods

        private DataTable GetDeviceDataFromGUID(string GUID)
        {
            using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectDeviceByGUID(GUID)))
            {
                return results;
            }
        }

        #endregion Methods
    }
}

namespace AssetManager
{
    public class RequestObject : DataMappingObject
    {
        #region Fields

        public DataTable RequestItems;

        #endregion Fields

        #region Constructors

        public RequestObject()
        {
            this.GUID = System.Guid.NewGuid().ToString();
        }

        public RequestObject(object data) : base(data)
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

namespace AssetManager
{
    public class DeviceHistoricalObject : DataMappingObject
    {
        #region Constructors

        public DeviceHistoricalObject()
        {
        }

        public DeviceHistoricalObject(object data) : base(data)
        {
        }

        #endregion Constructors

        #region Properties

        [DataColumnName(HistoricalDevicesCols.ActionDateTime)]
        public System.DateTime ActionDateTime { get; set; }

        [DataColumnName(HistoricalDevicesCols.ActionUser)]
        public string ActionUser { get; set; }

        [DataColumnName(HistoricalDevicesCols.ChangeType)]
        public string ChangeType { get; set; }

        [DataColumnName(HistoricalDevicesCols.HistoryEntryUID)]
        public override string GUID { get; set; }

        [DataColumnName(HistoricalDevicesCols.Notes)]
        public string Note { get; set; }

        public override string TableName { get; set; } = HistoricalDevicesCols.TableName;

        #endregion Properties
    }
}

namespace AssetManager
{
    public class DeviceTrackingObject : DataMappingObject
    {
        #region Constructors

        public DeviceTrackingObject()
        {
        }

        public DeviceTrackingObject(object data) : base(data)
        {
        }

        #endregion Constructors

        #region Properties

        [DataColumnName(TrackablesCols.Notes)]
        public string CheckinNotes { get; set; }

        [DataColumnName(TrackablesCols.CheckinTime)]
        public System.DateTime CheckinTime { get; set; }

        [DataColumnName(TrackablesCols.CheckinUser)]
        public string CheckinUser { get; set; }

        [DataColumnName(TrackablesCols.CheckoutTime)]
        public System.DateTime CheckoutTime { get; set; }

        [DataColumnName(TrackablesCols.CheckoutUser)]
        public string CheckoutUser { get; set; }

        [DataColumnName(TrackablesCols.DueBackDate)]
        public System.DateTime DueBackTime { get; set; }

        [DataColumnName(TrackablesCols.DeviceUID)]
        public override string GUID { get; set; }

        [DataColumnName(DevicesCols.CheckedOut)]
        public bool IsCheckedOut { get; set; }

        public override string TableName { get; set; } = TrackablesCols.TableName;
        [DataColumnName(TrackablesCols.UseLocation)]
        public string UseLocation { get; set; }

        [DataColumnName(TrackablesCols.Notes)]
        public string UseReason { get; set; }

        #endregion Properties
    }
}

namespace AssetManager
{
    public class AccessGroupObject : DataMappingObject
    {
        #region Constructors

        public AccessGroupObject()
        {
        }

        public AccessGroupObject(object data) : base(data)
        {
        }

        #endregion Constructors

        #region Properties

        [DataColumnName(SecurityCols.SecModule)]
        public string AccessModule { get; set; }

        [DataColumnName(SecurityCols.AvailOffline)]
        public bool AvailableOffline { get; set; }

        [DataColumnName(SecurityCols.Description)]
        public string Description { get; set; }

        public override string GUID { get; set; }
        [DataColumnName(SecurityCols.AccessLevel)]
        public int Level { get; set; }

        public override string TableName { get; set; } = SecurityCols.TableName;

        #endregion Properties
    }
}