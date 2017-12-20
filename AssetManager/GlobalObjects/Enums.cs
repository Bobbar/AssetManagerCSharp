namespace AssetManager
{
    public enum EntryType
    {
        Sibi,
        Device
    }
}

namespace AssetManager
{
    public enum PdfFormType
    {
        InputForm,
        TransferForm,
        DisposeForm
    }
}

namespace AssetManager
{
    public enum LiveBoxType
    {
        DynamicSearch,
        InstaLoad,
        SelectValue,
        UserSelect
    }
}

namespace AssetManager
{
    public enum FindDevType
    {
        AssetTag,
        Serial
    }
}

namespace AssetManager
{
    public enum CommandArgs
    {
        TESTDB,
        VINTONDD
    }
}

namespace AssetManager
{
    public enum Databases
    {
        test_db,
        asset_manager,
        vintondd
    }
}

namespace AssetManager
{
    public enum ItemChangeStatus
    {
        /// <summary>
        /// Is current. (Up-to-date)
        /// </summary>
        MODCURR,
        /// <summary>
        /// Is a new item.
        /// </summary>
        MODNEW,
        /// <summary>
        /// Is changed. Pending approval.
        /// </summary>
        MODCHAN,
        /// <summary>
        /// Status change. Only notify approver.
        /// </summary>
        MODSTCH
    }
}

namespace AssetManager
{
    public enum NotificationType
    {
        /// <summary>
        /// Notify approver of new approval.
        /// </summary>
        APPROVAL,
        /// <summary>
        /// Approver accepted. Notify approver and requestor.
        /// </summary>
        ACCEPTED,
        /// <summary>
        /// Approver rejected. Notify approver and requestor.
        /// </summary>
        REJECTED,
        /// <summary>
        /// Item status change. Only notify approver.
        /// </summary>
        CHANGE
    }
}
