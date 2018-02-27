using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetManager.Data.Classes;

namespace AssetManager.UserInterface.CustomControls.LiveBox
{
    public interface ILiveBox
    {
        void LoadDevice(string deviceGUID);

        void DynamicSearch();

        MunisEmployee MunisUser
        {
            get;

            set;
        }
    }
}
