using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint.Administration;

using Microsoft.Practices.SharePoint.Common.Logging;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;

namespace Treasury.ECM.eCase.SusDeb.DOI.Logging
{
    public class Logger
    {
        #region Properties
        public ILogger SPLogger;
        static private Logger _instance = null;
        static private readonly object _syncLock = new Object();
        private string _area = DiagnosticsAreas.ECASE_AREA;
        private DiagnosticsCategories _category = DiagnosticsCategories.General;
        private int _eventId = 65000;

        /// <summary>
        /// Default Area Property is "Treasury.ECM.DOI"
        /// Default Category Property is "General"
        /// Default EventId Property is 65000
        /// </summary>
        public static Logger Instance
        {
            get
            {
                lock (_syncLock)
                {
                    if (_instance == null)
                        _instance = new Logger();

                    return _instance;
                }
            }
        }
       
        #endregion 

        /// <summary>
        /// Private constructor invoked by Instance property
        /// </summary>
        /// <param name="diagnosticArea"></param>
        private Logger() { SPLogger = SharePointServiceLocator.GetCurrent().GetInstance<ILogger>(); }

        #region Area/Category Format
        public string AreaCategory() { return string.Format("{0}/{1}", _area, Enum.GetName(typeof(DiagnosticsCategories), _category)); }

        public string AreaCategory(DiagnosticsCategories category)
        {
            return string.Format("{0}/{1}", _area, Enum.GetName(typeof(DiagnosticsCategories), category));
        }

        public string AreaCategory(string area, DiagnosticsCategories category) { return string.Format("{0}/{1}", area, Enum.GetName(typeof(DiagnosticsCategories), category)); }
        #endregion

        #region Debug
        /// <summary>
        /// Uses default Area, Category, EventId
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message) { Debug(message, _category); }

        /// <summary>
        /// Uses default Area, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        public void Debug(string message, DiagnosticsCategories category)
        {
            SPLogger.TraceToDeveloper(message, _eventId, TraceSeverity.None, AreaCategory(category));
        }

        /// <summary>
        /// Uses default Area, Category, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Debug(string message, Exception exception) 
        { SPLogger.TraceToDeveloper(exception, message, _eventId, TraceSeverity.Unexpected, AreaCategory()); }

        /// <summary>
        /// Uses default Area
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="category"></param>
        public void Debug(string message, Exception exception, DiagnosticsCategories category)
        {
            SPLogger.TraceToDeveloper(exception, message, _eventId, TraceSeverity.None, AreaCategory(category));
        }
        #endregion

        #region Info
        /// <summary>
        /// Uses default Area, Category, EventId
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message) { Info(message, _category); }

        /// <summary>
        /// Uses default Area, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        public void Info(string message, DiagnosticsCategories category)
        {
            SPLogger.TraceToDeveloper(message, _eventId, TraceSeverity.Verbose, AreaCategory(category));
        }

        /// <summary>
        /// Uses default Area, Category, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Info(string message, Exception exception) 
        { SPLogger.TraceToDeveloper(exception, message, _eventId, TraceSeverity.Unexpected, AreaCategory()); }

        /// <summary>
        /// Uses default Area, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="category"></param>
        public void Info(string message, Exception exception, DiagnosticsCategories category)
        {
            SPLogger.TraceToDeveloper(exception, message, _eventId, TraceSeverity.Verbose, AreaCategory(category));
        }
        #endregion

        #region Warning
        /// <summary>
        /// Uses default Area, Category, EventId
        /// </summary>
        /// <param name="message"></param>
        public void Warning(string message) { Warning(message, _category); }

        /// <summary>
        /// Uses default Area, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        public void Warning(string message, DiagnosticsCategories category)
        {
            SPLogger.TraceToDeveloper(message, _eventId, TraceSeverity.Monitorable, AreaCategory(category));
        }

        /// <summary>
        /// Uses default Area, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Warning(string message, Exception exception)
        { SPLogger.TraceToDeveloper(exception, message, _eventId, TraceSeverity.Unexpected, AreaCategory()); }

        /// <summary>
        /// Uses default Area, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="category"></param>
        public void Warning(string message, Exception exception, DiagnosticsCategories category)
        {
            SPLogger.TraceToDeveloper(exception, message, _eventId, TraceSeverity.Monitorable, AreaCategory(category));
        }
        #endregion

        #region Error
        /// <summary>
        /// Uses default Area, Category, EventId
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message) { Error(message, _category); }

        /// <summary>
        /// Uses default Area, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        public void Error(string message, DiagnosticsCategories category)
        {
            SPLogger.TraceToDeveloper(message, _eventId, TraceSeverity.Unexpected, AreaCategory(category));
        }

        /// <summary>
        /// Uses default Area, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(string message, Exception exception)
        { SPLogger.TraceToDeveloper(exception, message, _eventId, TraceSeverity.Unexpected, AreaCategory()); }

        /// <summary>
        /// Uses default Area, EventId
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="category"></param>
        public void Error(string message, Exception exception, DiagnosticsCategories category)
        {
            SPLogger.TraceToDeveloper(exception, message, _eventId, TraceSeverity.Unexpected, AreaCategory(category));
        }
        #endregion
    }
}
