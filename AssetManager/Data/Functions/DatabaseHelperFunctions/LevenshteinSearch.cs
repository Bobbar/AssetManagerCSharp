using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AssetManager.Data.Functions
{

    /// <summary>
    /// Contains functions for performing accurate employee name searches using a fast Levenshtein algorithm.
    /// </summary>
    public static class LevenshteinSearch
    {
        /// <summary>
        /// Searches the database for the best possible match to the specified search name using a Levenshtein distance algorithm.
        /// </summary>
        /// <param name="searchName"></param>
        /// <param name="minSearchDistance"></param>
        /// <returns></returns>
        public static MunisEmployee SmartEmployeeSearch(string searchName, int minSearchDistance = 10)
        {
            if (searchName.Trim() != "")
            {
                // Split the name string by spaces to try and separate first/last names.
                string[] splitName = searchName.Split(char.Parse(" "));

                // Init new list of search result objects.
                var results = new List<SmartEmpSearchInfo>();

                // Get results for complete name from employees table
                results.AddRange(GetEmpSearchResults(EmployeesCols.TableName, searchName, EmployeesCols.Name, EmployeesCols.Number));

                // Get results for complete name from devices table
                results.AddRange(GetEmpSearchResults(DevicesCols.TableName, searchName, DevicesCols.CurrentUser, DevicesCols.MunisEmpNum));

                foreach (string s in splitName)
                {
                    //Get results for partial name from employees table
                    results.AddRange(GetEmpSearchResults(EmployeesCols.TableName, s, EmployeesCols.Name, EmployeesCols.Number));

                    //Get results for partial name from devices table
                    results.AddRange(GetEmpSearchResults(DevicesCols.TableName, s, DevicesCols.CurrentUser, DevicesCols.MunisEmpNum));
                }

                if (results.Count > 0)
                {
                    results = NarrowResults(results);
                    var BestMatch = FindBestSmartSearchMatch(results);
                    if (BestMatch.MatchDistance < minSearchDistance)
                    {
                        return BestMatch.SearchResult;
                    }
                }
            }
            return new MunisEmployee();
        }

        /// <summary>
        /// Reprocesses the search results to obtain a more accurate Levenshtein distance calculation.
        /// </summary>
        /// <param name="results"></param>
        /// <remarks>This is done because the initial calculations are performed against the full length
        /// of the returned names (First and last name), and the distance between the search string and name string may be inaccurate.</remarks>
        /// <returns></returns>
        private static List<SmartEmpSearchInfo> NarrowResults(List<SmartEmpSearchInfo> results)
        {
            var newResults = new List<SmartEmpSearchInfo>();

            //Iterate through results
            foreach (var result in results)
            {
                //Split the results returned string by spaces
                var resultSplit = result.SearchResult.Name.Split(char.Parse(" "));
                if (resultSplit.Count() > 0)
                {
                    //Iterate through the separate strings
                    foreach (var resultString in resultSplit)
                    {
                        //Make sure the result string contains the search string
                        if (resultString.Contains(result.SearchString) && resultString.StartsWith(result.SearchString))
                        {
                            //Get a new Levenshtein distance.
                            var newDistance = Fastenshtein.Levenshtein.Distance(resultString, result.SearchString);

                            //If the strings are closer together, add the new data.
                            if (newDistance < result.MatchDistance)
                            {
                                newResults.Add(new SmartEmpSearchInfo(result.SearchResult, result.SearchString, newDistance));
                            }
                            else
                            {
                                newResults.Add(result);
                            }
                        }
                        else
                        {
                            newResults.Add(result);
                        }
                    }
                }
            }
            return newResults;
        }

        /// <summary>
        /// Finds the best match within the results. The item with shortest Levenshtein distance and the longest match length (string length) is preferred.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private static SmartEmpSearchInfo FindBestSmartSearchMatch(List<SmartEmpSearchInfo> results)
        {
            //Initial minimum distance
            int minDist = results.First().MatchDistance;

            //Initial minimum match
            SmartEmpSearchInfo minMatch = results.First();
            var longestMatch = new SmartEmpSearchInfo();
            var deDupDist = new List<SmartEmpSearchInfo>();

            //Iterate through the results and determine the result with the shortest Levenshtein distance.
            foreach (var result in results)
            {
                if (result.MatchDistance < minDist)
                {
                    minDist = result.MatchDistance;
                    minMatch = result;
                }
            }

            //De-duplicate the results and iterate to determine which result of the Levenshtein shortest distances has the longest match length. (Greatest number of matches)
            deDupDist = results.Distinct().ToList();
            if (deDupDist.Count > 0)
            {
                int maxMatchLen = 0;
                foreach (var dup in deDupDist)
                {
                    if (dup.MatchDistance == minDist)
                    {
                        if (dup.MatchLength > maxMatchLen)
                        {
                            maxMatchLen = dup.MatchLength;
                            longestMatch = dup;
                        }
                    }
                }
                //Return best match by length and Levenshtein distance.
                return longestMatch;
            }
            //Return best match by Levenshtein distance only. (If no duplicates)
            return minMatch;
        }

        /// <summary>
        /// Queries the database for a list of results that contains the employee name result and computed Levenshtein distance to the search string.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="searchEmpName"></param>
        /// <param name="empNameColumn"></param>
        /// <param name="empNumColumn"></param>
        /// <returns></returns>
        private static List<SmartEmpSearchInfo> GetEmpSearchResults(string tableName, string searchEmpName, string empNameColumn, string empNumColumn)
        {
            var tmpResults = new List<SmartEmpSearchInfo>();
            var searchParams = new QueryParamCollection();

            // Parameterize the input.
            searchParams.Add(empNameColumn, searchEmpName, false);

            // Query the DB for the search value.
            using (DataTable data = DBFactory.GetDatabase().DataTableFromParameters("SELECT * FROM " + tableName + " WHERE", searchParams.Parameters))
            {
                foreach (DataRow row in data.Rows)
                {
                    var searchValue = searchEmpName.ToUpper();
                    var dbValue = row[empNameColumn].ToString().ToUpper();

                    // Get the distance between the search value and DB value.
                    var distance = Fastenshtein.Levenshtein.Distance(searchValue, dbValue);
                    var employee = new MunisEmployee(row[empNameColumn].ToString(), row[empNumColumn].ToString());

                    // Add new entry to the list.
                    tmpResults.Add(new SmartEmpSearchInfo(employee, searchEmpName, distance));
                }
            }
            return tmpResults;
        }
    }
}