using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Treasury.ECM.eCase.SusDeb.DOI.Logging
{
    public static class DiagnosticsAreas
    {
        public static readonly string ECASE_AREA = "Treasury.ECM.DOI";
    }

    public enum DiagnosticsCategories
    {
        eCaseCommon,
        eCaseExtensions,
        eCaseSearch,
        eCaseSite,
        eCaseWeb,
        General
    }
}
