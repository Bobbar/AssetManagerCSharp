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



                    for (int i = 0; i < distinctValues.Rows.Count; i++)
                    {
                        var row = distinctValues.Rows[i];
                        SetItemApprovals(approvals, request.GUID, row[SibiRequestItemsCols.ApproverId].ToString());

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
            //for (int i = 0; i < filterRows.Count(); i++)
            //{

            //    filterRows[i][SibiRequestItemsCols.ApprovalID] = approvalID;

            //}



        }

        //private static void SetItemApprovals(DataTable allApprovals, string requestId, string approverId, string requestorId)
        //{
        //    var filterRows = allApprovals.Select(SibiRequestItemsCols.ApproverId + " = " + approverId);

        //    var approvalID = AddNewApproval(requestId, approverId, requestorId);
        //    for (int i = 0; i < filterRows.Count(); i++)
        //    {
        //        filterRows[i][SibiRequestItemsCols.ApprovalID] = approvalID;

        //    }



        //}

        private static string AddNewApproval(string requestId, string approverId, string requestorId)
        {
            var newUID = Guid.NewGuid().ToString();
            var approvalsQuery = "SELECT * FROM sibi_request_items_approvals LIMIT 0";
            using (DataTable newApproval = DBFactory.GetDatabase().DataTableFromQueryString(approvalsQuery))
            {
                newApproval.Rows.Add();
                var newRow = newApproval.Rows[0];

                newRow["uid"] = newUID;
                newRow["sibi_request_uid"] = requestId;
                newRow["approval_status"] = "new";
                newRow["approver_id"] = approverId;
                newRow["requestor_id"] = requestorId;

                DBFactory.GetDatabase().UpdateTable(approvalsQuery, newApproval);



            }




            return newUID;
        }




    }
}
