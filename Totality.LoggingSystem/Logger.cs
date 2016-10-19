using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model.Interfaces;
using NLog;
using NLog.Targets;
using NLog.Config;
using NLog.Targets.Wrappers;

namespace Totality.LoggingSystem
{
    public class Logger : Model.Interfaces.ILogger
    {
        private NLog.Logger _log = LogManager.GetCurrentClassLogger();
        private AsyncTargetWrapper _wrapper;
        private LoggingWindow _logWindow = new LoggingWindow();

        public Logger()
        {

            var target = new WpfRichTextBoxTarget
            {
                Name = "WpfConsole",
                Layout = "${time}|${level}|${message}",
                ControlName = _logWindow.logOutBox.Name,
                FormName = _logWindow.Name,
                AutoScroll = true,
                MaxLines = 100000,
                UseDefaultRowColoringRules = true
            };
            _wrapper = new AsyncTargetWrapper
            {
                Name = "WpfConsole",
                WrappedTarget = target
            };

            var target2 = new FileTarget
            {
                Name = "WpfFile",
                FileName = "${basedir}/logs/${shortdate}.txt",
                Layout = "${time}|${level}|${message}",
            };
            var _wrapper2 = new AsyncTargetWrapper
            {
                Name = "WpfFile",
                WrappedTarget = target2
            };

            LoggingConfiguration config = new LoggingConfiguration();
            config.AddTarget(_wrapper);
            config.AddTarget(_wrapper2);
            config.AddRuleForAllLevels(_wrapper);
            config.AddRuleForAllLevels(_wrapper2);
            LogManager.Configuration = config;



            //SimpleConfigurator.ConfigureForTargetLogging(_wrapper, LogLevel.Trace);
            //SimpleConfigurator.ConfigureForTargetLogging(_wrapper2, LogLevel.Trace);

            _log.Info("                                 LOGGING STARTED");
        }

        public void Error(string msg)
        {
            _log.Error(msg);
        }

        public void Fatal(string msg)
        {
            _log.Fatal(msg);
        }

        public void Info(string msg)
        {
            _log.Info(msg);
        }

        public void Trace(string msg)
        {
            _log.Trace(msg);
        }

        public void Warn(string msg)
        {
            _log.Warn(msg);
        }

        public void showLoggingWindow()
        {
            _logWindow.Show();
        }

        public void killLoggingWindow()
        {
            _logWindow.CloseBackground();
        }

    }
}
