using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptionsPractice.Models
{
    [Serializable]
    public class AppInfoOptions
    {
        public string AppName { get; set; }
        public string AppVersion { get; set; }
    }
}
