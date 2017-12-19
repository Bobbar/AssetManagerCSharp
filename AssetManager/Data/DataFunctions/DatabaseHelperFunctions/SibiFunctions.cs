using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AssetManager.Data.DataFunctions.DatabaseHelperFunctions
{
    public static class SibiFunctions
    {
        public static void ProcessApprovals(SibiRequestMapObject request)
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
                     
                      //  SetItemApprovals(approvals, request.GUID, row[SibiRequestItemsCols.ApproverId].ToString());

                    }

                }



            }



        }


        private static void AddNewApprovalItems(string requestId, string approvalId, string approverId)
        {
            var requestItemsQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.RequestUID + " = '" + requestId + "' AND " + SibiRequestItemsCols.ApproverId + " = '" + approverId + "'";

            var approvalItemsQuery = "SELECT * FROM " + SibiApprovalItemsColumns.TableName + " LIMIT 0";






            using (DataTable newItems = DBFactory.GetDatabase().DataTableFromQueryString(approvalItemsQuery))
            using (DataTable requestItems = DBFactory.GetDatabase().DataTableFromQueryString(requestItemsQuery))
            {
                if (requestItems.Rows.Count > 0)
                {

                    foreach (DataRow row in requestItems.Rows)
                    {
                        newItems.Rows.Add();



                    }

                }

               

            }




        }


        private static void SetItemApprovals(DataTable allApprovals, string requestId, string approverId)
        {
            var approvalQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.RequestUID + " = '" + requestId + "' AND " + SibiRequestItemsCols.RequiresApproval + " = '1'";

            var filterRows = allApprovals.Select(SibiRequestItemsCols.ApproverId + " = " + approverId);
            var requestorId = filterRows[0][SibiRequestItemsCols.RequestorId].ToString();

            var approvalID = AddNewApproval(requestId, approverId, requestorId);

            for (int i = 0; i < allApprovals.Rows.Count; i++)
            {
                var row = allApprovals.Rows[i];
                if (row[SibiRequestItemsCols.ApproverId].ToString() == approverId)
                {
                    row[SibiRequestItemsCols.ApprovalID] = approvalID;
                }


            }


            DBFactory.GetDatabase().UpdateTable(approvalQuery, allApprovals);
           



        }

    
        private static string AddNewApproval(string requestId, string approverId, string requestorId)
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

                DBFactory.GetDatabase().UpdateTable(approvalsQuery, newApproval);



            }




            return newUID;
        }




    }
}
