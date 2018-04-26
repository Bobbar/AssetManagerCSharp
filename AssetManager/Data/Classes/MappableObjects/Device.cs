using AssetManager.Data.Communications;
using System.Data;

namespace AssetManager.Data.Classes
{
    public class Device : MappableObject
    {
        #region Constructors

        public Device()
        {
        }

        public Device(DataTable data) : base(data)
        {
        }

        public Device(string guid)
        {
            this.MapClassProperties(GetDeviceDataFromGuid(guid));
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

        [DataColumnName(DevicesCols.DeviceGuid)]
        public override string Guid { get; set; }

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

        [DataColumnName(DevicesCols.SibiLinkGuid)]
        public string SibiLink { get; set; }

        [DataColumnName(DevicesCols.Status)]
        public string Status { get; set; }

        public override string TableName { get; set; } = DevicesCols.TableName;

        public TrackingEntry Tracking { get; set; } = new TrackingEntry();

        #endregion Properties

        #region Methods

        private DataTable GetDeviceDataFromGuid(string guid)
        {
            using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectDeviceByGuid(guid)))
            {
                results.TableName = DevicesCols.TableName;
                return results;
            }
        }

        #endregion Methods
    }
}
