using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceNetCore.Models
{
   
    public class Settings
    {
        private readonly IConfiguration confiration;

        public Settings(IConfiguration confiration)
        {
            this.confiration = confiration;
        }
        public int WorkerCount => confiration.GetValue<int>("WorkersCount");
        public int RunInterval => confiration.GetValue<int>("RunInterval");
        public string InstanceName => confiration.GetValue<string>("InstanceName");
        public string ResultPath => confiration.GetValue<string>("ResultPath");
    }
}
