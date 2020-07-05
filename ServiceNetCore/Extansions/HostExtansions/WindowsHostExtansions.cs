using Microsoft.Extensions.Hosting;
using ServiceNetCore.WindowsService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceNetCore.Extansions.HostExtansions
{
    public static class WindowsHostExtansions
    {
        public static async Task RunService(this IHostBuilder hostBuilder)
        {
            if(!Environment.UserInteractive)
            {
                await hostBuilder.RunAsServiceAsync();
            }
            else
            {
                await hostBuilder.RunConsoleAsync();
            }
        }
    }
}
