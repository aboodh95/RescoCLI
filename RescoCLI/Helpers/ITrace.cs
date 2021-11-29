using System;
using System.Collections.Generic;
using System.Text;

namespace RescoCLI.Helpers
{
    public interface ITrace
    {
        void WriteLine(string format, params object[] args);
        void Write(string format, params object[] args);
    }
}
