using Microsoft.Office.Server.Search.WebControls;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search
{
    /// <summary>
    /// Read in all fql created scopes
    /// Used for building fql with the correct data types
    /// This source code is released under the MIT license
    /// The code is based on code from http://neganov.blogspot.com/2011/01/extending-coreresultswebpart-to-handle.html
    /// </summary>
    public class eCaseCoreResultsDataSource : CoreResultsDatasource
    {
        internal string DuplicateTrimProperty;
        internal bool EnableFql;
        private const string CoreFqlResultsViewName = "CoreFqlResults";

        public eCaseCoreResultsDataSource(CoreResultsWebPart parentWebPart, bool enableFql, string duplicateTrimProperty)
            : base(parentWebPart)
        {
            EnableFql = enableFql;
            DuplicateTrimProperty = duplicateTrimProperty;
            // Replace default view with a custom view.
            base.View = new eCaseCoreResultsDataSourceView(this, CoreFqlResultsViewName);            
        }
    }
}

