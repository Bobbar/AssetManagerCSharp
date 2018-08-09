using Databases.Data.Mapping;
using Databases.Data;

namespace AssetManager.Data.Classes
{
    public class TrackingEntry : MappedObject
    {
        #region Constructors

        public TrackingEntry()
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

        [DataColumnName(TrackablesCols.DeviceGuid)]
        public override string Guid { get; set; }

        [DataColumnName(DevicesCols.CheckedOut)]
        public bool IsCheckedOut { get; set; }

        public override string TableName { get; set; } = TrackablesCols.TableName;
        [DataColumnName(TrackablesCols.UseLocation)]
        public string UseLocation { get; set; }

        [DataColumnName(TrackablesCols.Notes)]
        public string UseReason { get; set; }

        public override IDatabase Database
        {
            get
            {
                return DBFactory.GetDatabase();
            }
        }

        #endregion Properties
    }
}
