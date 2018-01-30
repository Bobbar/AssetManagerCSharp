using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tools
{
    public static class MemoryTweaks
    {
        private const int maxSetSizeMB = 30;
        private const int minSetSizeMB = 10;

        public async static void SetWorkingSet()
        {
            await Task.Run(() =>
            {
                Task.Delay(1000).Wait();
                System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
                int maxSetInBytes = maxSetSizeMB * 1000000;
                int minSetInBytes = minSetSizeMB * 1000000;
                process.MaxWorkingSet = (IntPtr)maxSetInBytes;
                process.MinWorkingSet = (IntPtr)minSetInBytes;
            });
        }


    }
}
