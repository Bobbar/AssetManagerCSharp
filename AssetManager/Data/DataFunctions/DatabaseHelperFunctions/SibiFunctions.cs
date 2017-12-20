using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace AssetManager.Data.DataFunctions.DatabaseHelperFunctions
{
    public static class SibiFunctions
    {
        public static void ProcessApprovals(SibiRequestMapObject request)
        {
            using (var trans = DBFactory.GetDatabase().StartTransaction())
            {
                try
                {

                    var approvalQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.RequestUID + " = '" + request.GUID + "' AND " + SibiRequestItemsCols.RequiresApproval + " = '1'";
                    using (DataTable approvals = DBFactory.GetDatabase().DataTableFromQueryString(approvalQuery))
                    {
                        if (approvals.Rows.Count > 0)
                        {
                            DataView view = new DataView(approvals);
                            DataTable distinctValues = view.ToTable(true, SibiRequestItemsCols.ApproverId);

                            //Get unique approver IDs within items.
                            for (int i = 0; i < distinctValues.Rows.Count; i++)
                            {
                                var row = distinctValues.Rows[i];
                                var approverId = row[SibiRequestItemsCols.ApproverId].ToString();
                                AddNewApprovalItems(request.GUID, approverId, trans);
                            }
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            }
        }


        private static void AddNewApprovalItems(string requestId, string approverId, DbTransaction transaction)
        {

            bool hasApproval = false;
            string newApprovalID = string.Empty;

            // Select all sibi request items that require approval and match the request and approver ID.
            var requestItemsQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.RequestUID + " = '" + requestId + "' AND " + SibiRequestItemsCols.ApproverId + " = '" + approverId + "' AND " + SibiRequestItemsCols.RequiresApproval + " = '1'";

            // Select blank approval items table
            var approvalItemsQuery = "SELECT * FROM " + SibiApprovalItemsColumns.TableName + " LIMIT 0";



            using (DataTable newItems = DBFactory.GetDatabase().DataTableFromQueryString(approvalItemsQuery))
            using (DataTable requestItems = DBFactory.GetDatabase().DataTableFromQueryString(requestItemsQuery))
            {
                if (requestItems.Rows.Count > 0)
                {

                    // Iterate through the request items.
                    foreach (DataRow requestRow in requestItems.Rows)
                    {

                        // Check if the current item is in a state that requires an approval.
                        if (RequiresApproval(requestRow[SibiRequestItemsCols.ModifyStatus].ToString()))
                        {
                            // Create a new approval if we haven't already done so.
                            if (!hasApproval)
                            {
                                // Add a new approval and collect the new ID.
                                var requestorId = requestRow[SibiRequestItemsCols.RequestorId].ToString();
                                newApprovalID = AddNewApproval(requestId, approverId, requestorId, transaction);
                                // Add a new approval notification referencing the new ID.
                                AddNewNotification(NotificationType.APPROVAL, transaction, newApprovalID);
                                hasApproval = true;
                            }

                            // Add new approval items.
                            newItems.Rows.Add();
                            var newRow = newItems.Rows[newItems.Rows.Count - 1];
                            string requestItemUID;
                            object itemHistoryID = null;

                            newRow[SibiApprovalItemsColumns.UID] = Guid.NewGuid().ToString();
                            newRow[SibiApprovalItemsColumns.ApprovalUID] = newApprovalID;
                            newRow[SibiApprovalItemsColumns.RequestItemUID] = requestRow[SibiRequestItemsCols.ItemUID];
                            // Collect request item UID for use in history ID query.
                            requestItemUID = requestRow[SibiRequestItemsCols.ItemUID].ToString();
                            newRow[SibiApprovalItemsColumns.ChangeType] = requestRow[SibiRequestItemsCols.ModifyStatus];
                            // Collect the most recent historical entry for the current request item.
                            itemHistoryID = DBFactory.GetDatabase().ExecuteScalarFromQueryString("SELECT MAX(" + SibiHistoricalItemsCols.HistID + ") FROM " + SibiHistoricalItemsCols.TableName + " WHERE " + SibiHistoricalItemsCols.ItemUID + " = '" + requestItemUID + "'");
                            newRow[SibiApprovalItemsColumns.ItemHistoryUID] = itemHistoryID;
                            // Serialize the current row and add it to the new values column for later reference.
                            newRow[SibiApprovalItemsColumns.NewValues] = SecurityTools.SerializeDataRow(requestRow);

                        }
                        // If the current item state only requires a notification.
                        else if (RequiresNotificationOnly(requestRow[SibiRequestItemsCols.ModifyStatus].ToString()))
                        {
                            // Add a simple notification.
                            AddNewNotification(NotificationType.CHANGE, transaction, null, requestRow[SibiRequestItemsCols.ItemUID].ToString());
                        }

                    }

                    // If we added new approval items, add them to the DB.
                    if (newItems.Rows.Count > 0)
                    {
                        DBFactory.GetDatabase().UpdateTable(approvalItemsQuery, newItems, transaction);
                    }
                }

            }

        }

        private static bool RequiresApproval(string changeTypeString)
        {
            ItemChangeStatus changeType = (ItemChangeStatus)Enum.Parse(typeof(ItemChangeStatus), changeTypeString);

            switch (changeType)
            {
                case ItemChangeStatus.MODNEW:
                case ItemChangeStatus.MODCHAN:
                    return true;
                default:
                    return false;
            }
        }

        private static bool RequiresNotificationOnly(string changeTypeString)
        {
            ItemChangeStatus changeType = (ItemChangeStatus)Enum.Parse(typeof(ItemChangeStatus), changeTypeString);

            switch (changeType)
            {
                case ItemChangeStatus.MODNEW:
                case ItemChangeStatus.MODCHAN:
                case ItemChangeStatus.MODCURR:
                    return false;
                default:
                    return true;
            }
        }


        private static string AddNewApproval(string requestId, string approverId, string requestorId, DbTransaction transaction)
        {
            var newUID = Guid.NewGuid().ToString();
            var approvalsQuery = "SELECT * FROM " + SibiApprovalColumns.TableName + " LIMIT 0";
            using (DataTable newApproval = DBFactory.GetDatabase().DataTableFromQueryString(approvalsQuery))
            {
                newApproval.Rows.Add();
                var newRow = newApproval.Rows[0];

                newRow[SibiApprovalColumns.UID] = newUID;
                newRow[SibiApprovalColumns.RequestUID] = requestId;
                newRow[SibiApprovalColumns.Status] = "pending";
                newRow[SibiApprovalColumns.ApproverID] = approverId;
                newRow[SibiApprovalColumns.RequestorID] = requestorId;

                DBFactory.GetDatabase().UpdateTable(approvalsQuery, newApproval, transaction);
            }
            return newUID;
        }

        private static void AddNewNotification(NotificationType type, DbTransaction transaction, string approvalId = "", string requestItemUID = "")
        {
            var newGUID = Guid.NewGuid().ToString();
            var emptyNotificationQry = "SELECT * FROM " + NotificationColumns.TableName + " LIMIT 0";
            using (var newNotification = DBFactory.GetDatabase().DataTableFromQueryString(emptyNotificationQry))
            {
                newNotification.Rows.Add();
                var newRow = newNotification.Rows[0];

                newRow[NotificationColumns.UID] = newGUID;

                switch (type)
                {
                    case NotificationType.APPROVAL:
                        newRow[NotificationColumns.Type] = NotificationType.APPROVAL.ToString();
                        newRow[NotificationColumns.ApprovalID] = approvalId;

                        break;

                    case NotificationType.ACCEPTED:
                        newRow[NotificationColumns.Type] = NotificationType.ACCEPTED.ToString();
                        newRow[NotificationColumns.ApprovalID] = approvalId;

                        break;

                    case NotificationType.REJECTED:
                        newRow[NotificationColumns.Type] = NotificationType.REJECTED.ToString();
                        newRow[NotificationColumns.ApprovalID] = approvalId;

                        break;

                    case NotificationType.CHANGE:
                        newRow[NotificationColumns.Type] = NotificationType.CHANGE.ToString();
                        newRow[NotificationColumns.RequestItemUID] = requestItemUID;

                        break;
                }

                DBFactory.GetDatabase().UpdateTable(emptyNotificationQry, newNotification, transaction);

            }
        }



    }
}
