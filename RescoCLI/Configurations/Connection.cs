using System;
using System.Collections.Generic;
using System.Text;

namespace RescoCLI.Configurations
{
    public class Connection
    {
        /// <summary>
        /// he URL of your org
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// Resco User Name
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Resco Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Is this connection selected to be used by default
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
