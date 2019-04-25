using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint.Administration;

using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.Practices.SharePoint.Common.Configuration;
using Microsoft.Practices.SharePoint.Common.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Logging
{
    public class LoggingManager
    {
        private IConfigManager configMgr;
        private DiagnosticsAreaCollection configuredAreas;
        public LoggingManager()
        {
            configMgr = SharePointServiceLocator.GetCurrent().GetInstance<IConfigManager>();
            configuredAreas = new DiagnosticsAreaCollection(configMgr);
        }

        public bool AddDiagnosticArea(DiagnosticsArea area)
        {
            bool retVal = false;

            var existingArea = configuredAreas[area.Name];

            if (existingArea == null)
                configuredAreas.Add(area);
            else
            {
                foreach (DiagnosticsCategory c in area.DiagnosticsCategories)
                {
                    var existingCategory = existingArea.DiagnosticsCategories[c.Name];
                    if (existingCategory == null)
                    {
                        existingArea.DiagnosticsCategories.Add(c);
                        retVal = true;
                    }
                }
            }

            return retVal;
        }

        public bool RemoveDiagnosticArea(DiagnosticsArea area)
        {
            bool retVal = false;
            DiagnosticsArea areaToRemove = configuredAreas[area.Name];
            if (areaToRemove != null)
            {
                foreach (DiagnosticsCategory c in area.DiagnosticsCategories)
                {
                    var existingCat = areaToRemove.DiagnosticsCategories[c.Name];
                    if (existingCat != null)
                        areaToRemove.DiagnosticsCategories.Remove(existingCat);
                }
                if (areaToRemove.DiagnosticsCategories.Count == 0)
                    configuredAreas.Remove(areaToRemove);
            }
            return retVal;
        }

        public void Save()
        {
            configuredAreas.SaveConfiguration();
        }

    }
}
