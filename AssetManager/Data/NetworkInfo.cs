﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetManager.Security;

namespace AssetManager.Data
{
    public static class NetworkInfo
    {
        public static string LocalDomainUser { get; set; }

        private static string _currentDomain = "core.co.fairfield.oh.us";
        public static string CurrentDomain
        {
            get { return _currentDomain; }
        }

        private static Dictionary<DatabaseName, string> DomainNames = new Dictionary<DatabaseName, string>
        {
            {DatabaseName.test_db, "core.co.fairfield.oh.us"},
            {DatabaseName.asset_manager, "core.co.fairfield.oh.us"},
            {DatabaseName.vintondd, "vinton.local"}
        };

        private static Dictionary<string, string> SubnetLocations = new Dictionary<string, string>
        {
            {"10.10.80.0", "Admin"},
            {"10.10.81.0", "OC"},
            {"10.10.82.0", "DiscoverU"},
            {"10.10.83.0", "FRS"},
            {"10.10.84.0", "PRO"},
            {"10.10.85.0", "Art & Clay"},
            {"10.17.80.0", "Admin - Wifi"},
            {"10.17.81.0", "OC - Wifi"},
            {"10.17.83.0", "FRS - Wifi"},
            {"10.17.84.0", "PRO - Wifi"},
            {"10.11.80.0", "VPN-DDAD"}
        };

        public static string LocationOfIP(string ip)
        {
            try
            {
                string subnet = ip.Substring(0, 8) + ".0";
                if (SubnetLocations.ContainsKey(subnet))
                {
                    return SubnetLocations[subnet];
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string SetCurrentDomain(DatabaseName database)
        {
            _currentDomain = DomainNames[database];
            SecurityTools.ClearAdminCreds();
            if (database == DatabaseName.vintondd)
            {
                SecurityTools.VerifyAdminCreds("Credentials for Vinton AD");
            }
            return DomainNames[database];
        }
    }
}
