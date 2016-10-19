using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model.Interfaces;
using NLog;

namespace Totality.LoggingSystem
{
    public class Logger : Model.Interfaces.ILogger
    {
        private NLog.Logger _log = LogManager.GetCurrentClassLogger();

        public Logger()
        {
            _log.Info("                                 LOGGING STARTED");
        }

        public void Error(string msg)
        {
            _log.Error("            " + msg);
        }

        public void Fatal(string msg)
        {
            _log.Fatal("        !!!!!!!!!!!!" + msg + "!!!!!!!!!!!!");
        }

        public void Info(string msg)
        {
            _log.Info("   " + msg);
        }

        public void Trace(string msg)
        {
            _log.Trace(msg);
        }

        public void Warn(string msg)
        {
            _log.Warn("     " + msg);
        }
    }
}
