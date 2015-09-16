/*
	'=======================================================================
	'   Author(s):      D V Ra
	'   Module/Form:    LogHelper.cs
	'   Created Date:   
	'   Description:    LogHelper.cs
	'
	'   Modification History:
	'=======================================================================
	'   Author(s)       Date        Control/Procedure       Change
	'=======================================================================
	'=======================================================================
	*/

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.Text;

namespace VistaSG.Common
{
    public class LogHelper
    {
        private ILog _log = null;
        private string _logName = string.Empty;
        private string _appenderName = string.Empty;

        public ILog Log
        {
            get
            {
                return _log;
            }
        }
        public LogHelper(string logFileName, string logLevelText, string logName, bool rollingFile)
        {
            Level logLevel = Level.Info;
            if (!string.IsNullOrWhiteSpace(logLevelText))
            {
                if (logLevelText.Equals("debug", StringComparison.OrdinalIgnoreCase))
                {
                    logLevel = Level.Debug;
                }
                else if (logLevelText.Equals("critical", StringComparison.OrdinalIgnoreCase))
                {
                    logLevel = Level.Critical;
                }
                else if (logLevelText.Equals("error", StringComparison.OrdinalIgnoreCase))
                {
                    logLevel = Level.Error;
                }
                else if (logLevelText.Equals("warning", StringComparison.OrdinalIgnoreCase))
                {
                    logLevel = Level.Warn;
                }
            }
            this.Initialize(logFileName, logLevel, logName, rollingFile);
        }

        public LogHelper(string logFileName, Level threshold, string logName, bool rollingFile)
        {
            this.Initialize(logFileName, threshold, logName, rollingFile);
        }

        private void Initialize(string logFileName, Level threshold, string logName, bool rollingFile)
        {
            _logName = logName;
            string appenderName = string.Format("{0}Appender" , logName) ;
            _appenderName = appenderName;
            var fileAppender = rollingFile ? GetRollingFileAppender(logFileName, threshold , appenderName) : GetFileAppender(logFileName, threshold , appenderName);
            BasicConfigurator.Configure(fileAppender);
            ((Hierarchy)LogManager.GetRepository()).Root.Level = threshold;
            _log = LogManager.GetLogger(logName);
            Logger logger = _log.Logger as Logger;
            logger.Additivity = false;
            logger.AddAppender(fileAppender);
        }
        public void CloseLog()
        {
            if (_log != null)
            {
                foreach (log4net.Appender.IAppender app in _log.Logger.Repository.GetAppenders())
                {
                    if ( !string.IsNullOrWhiteSpace(app.Name) && 
                         !string.IsNullOrWhiteSpace(_appenderName) &&
                         app.Name.Equals(_appenderName, StringComparison.OrdinalIgnoreCase))
                    {
                        app.Close();
                    }
                }
                _log = null;
            }
        }
        private IAppender GetFileAppender(string logFile, Level threshold, string appenderName)
        {
            var layout = new PatternLayout("%date %-5level %logger - %message%newline");
            layout.ActivateOptions(); // According to the docs this must be called as soon as any properties have been changed.

            var appender = new FileAppender
            {
                File = logFile,
                Encoding = Encoding.UTF8,
                Threshold = threshold,
                Layout = layout ,
                 Name = appenderName
            };

            appender.ActivateOptions();

            return appender;
        }

        private IAppender GetRollingFileAppender(string logFile, Level threshold , string appenderName)
        {
            var layout = new PatternLayout("%date %-5level %logger - %message%newline");
            layout.ActivateOptions(); // According to the docs this must be called as soon as any properties have been changed.

            var appender = new RollingFileAppender
            {
                File = logFile,
                Encoding = Encoding.UTF8,
                Threshold = threshold,
                Layout = layout,
                StaticLogFileName = false,
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Composite,
                DatePattern = "_yyyy_MM_dd",
                PreserveLogFileNameExtension = true ,
                Name = appenderName
            };

            appender.ActivateOptions();

            return appender;
        }

        public void LogToTrace(TraceEventType traceEventType, string errorMessage)
        {
            if (_log != null)
            {
                if (traceEventType == TraceEventType.Critical)
                {
                    _log.Fatal(errorMessage);
                }
                else if (traceEventType == TraceEventType.Error)
                {
                    _log.Error(errorMessage);
                }
                else if (traceEventType == TraceEventType.Warning)
                {
                    _log.Warn(errorMessage);
                }
                else if (traceEventType == TraceEventType.Information)
                {
                    _log.Info(errorMessage);
                }
                if (traceEventType == TraceEventType.Verbose)
                {
                    _log.Debug(errorMessage);
                }
            }

        }
    }
}
