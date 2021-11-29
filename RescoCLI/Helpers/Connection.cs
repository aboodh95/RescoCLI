using System;
using System.Collections.Generic;
using System.Text;

namespace RescoCLI.Helpers
{
    public class Connection
    {
        public string URL { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
