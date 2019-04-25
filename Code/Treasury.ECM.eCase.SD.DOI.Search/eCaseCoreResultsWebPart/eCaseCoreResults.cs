using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using Microsoft.Office.Server.Search.WebControls;
using Microsoft.SharePoint;
using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;

using Microsoft.Office.Server.Search.Administration;
using Treasury.ECM.eCase.SusDeb.DOI.Search.FAST;
using Treasury.ECM.eCase.SusDeb.DOI.Search.KqlParser;


namespace Treasury.ECM.eCase.SusDeb.DOI.Search
{
    /// <summary>
    /// FQL and synonym enabled web part
    /// Used for building fql with the correct data types
    /// This source code is released under the MIT license
    /// The code is copied from http://neganov.blogspot.com/2011/01/extending-coreresultswebpart-to-handle.html
    /// </summary>
    [ToolboxItem(false)]
    public class eCaseCoreResults : CoreResultsWebPart
    {
        private static Regex _reNonCharacter = new Regex(@"\W", RegexOptions.Compiled);
        private string _query;
        private Dictionary<string, List<string>> _synonymLookup;
        private bool _enableFql = true;
        private string _cacheKey;
        private int _cacheMinutes = 60;
        private int _boostValue = 500;
        private string _duplicateTrimProperty = "DocumentSignature";
        private SynonymHandling _synonymHandling = SynonymHandling.Include;

        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(true)]
        [Category("Advanced Query Options")]
        [WebDisplayName("Synonym handling")]
        [WebDescription("Choose to expand synonyms")]
        public SynonymHandling SynonymHandling
        {
            get { return _synonymHandling; }
            set { _synonymHandling = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(true)]
        [Category("Advanced Query Options")]
        [WebDisplayName("Query Language")]
        [WebDescription("Kql or Fql")]
        [DefaultValue(QueryKind.Kql)]
        public QueryKind QueryKind { get; set; }

        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(true)]
        [Category("Advanced Query Options")]
        [WebDisplayName("Original Query Boost Value")]
        [WebDescription("Boost the original entered query")]
        public int BoostValue
        {
            get { return _boostValue; }
            set { _boostValue = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(true)]
        [Category("Advanced Query Options")]
        [WebDisplayName("Cache time for synonyms and scopes")]
        [WebDescription("Cache the values for specified minutes. 0=no caching")]
        public int CacheMinutes
        {
            get { return _cacheMinutes; }
            set { _cacheMinutes = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(true)]
        [Category("Advanced Query Options")]
        [WebDisplayName("Duplicate Trimming Property")]
        [WebDescription("Trim duplicates on a custom managed property")]
        public string DuplicateTrimProperty
        {
            get { return _duplicateTrimProperty; }
            set { _duplicateTrimProperty = value; }
        }

        public override string Xsl
        {
            get
            {
                string xsltRelUrl;
                // Not sure how the Page can go null, but it does...
                if (Page != null && Page.Request != null && !string.IsNullOrEmpty(Page.Request.QueryString["x"]))
                    xsltRelUrl = Page.Request.QueryString["x"];
                else
                    xsltRelUrl = "ecrDefault.xslt";
                if (!string.IsNullOrEmpty(xsltRelUrl) && SPContext.Current != null)
                {
                    try
                    {
                        xsltRelUrl = string.Format("{0}/Pages/Xslt/{1}", SPContext.Current.Web.Url, xsltRelUrl);
                        SPFile xslt = SPContext.Current.Web.GetFile(xsltRelUrl);
                        if (xslt.Exists)
                            this.Xsl = xslt.GetContents(); // TODO: NEED TO CACHE THIS DATA
                    }
                    catch (Exception x)
                    {
                        Logger.Instance.Error(string.Format("Failed to load {0} in Search Center", xsltRelUrl), x, DiagnosticsCategories.eCaseSearch);
                    }
                }
                return this._xsl;
            }
            set
            {
                if (this._xsl != null && this._xsl != value && (value == null || value.Trim() != this._xsl.Trim()))
                {
                    this.ResetXslCache();
                }
                if (value != null && value.Length > 0)
                    _xsl = value;
                else
                    _xsl = string.Empty;
            }
        }

        private DropDownList _xsltDropDownList;

        protected override void OnPreRender(EventArgs e)
        {
            if (!Page.IsPostBack)
                PopulateXsltDropDownList();
            
            base.OnPreRender(e);
        }

        protected override void CreateChildControls()
        {
            _xsltDropDownList = new DropDownList();
            _xsltDropDownList.AutoPostBack = true;
            _xsltDropDownList.EnableViewState = true;
            _xsltDropDownList.SelectedIndexChanged += _xsltDropDownList_SelectedIndexChanged;
            _xsltDropDownList.CssClass = "ecrXsltDdl";
            this.Controls.Add(_xsltDropDownList);
            base.CreateChildControls();
        }

        void _xsltDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                string request = Page.Request.RawUrl;
                int start = request.IndexOf("&x=");
                if (start > -1)
                {
                    string temp = string.Empty;
                    int end = request.IndexOf("&", start+1);
                    if (end > -1) //&& start != end)
                        temp = request.Substring(end);
                    request = request.Substring(0, start) + temp;
                }
                request += string.Format("&x={0}", _xsltDropDownList.SelectedItem.Value);
                Page.Response.Redirect(request);
            }
        }

        private void PopulateXsltDropDownList()
        {
            SPList pagesList = SPContext.Current.Web.Lists["Pages"];
            SPFolder xsltFolder = pagesList.RootFolder.SubFolders["Xslt"];
            foreach (SPFile file in xsltFolder.Files)
                _xsltDropDownList.Items.Add(new ListItem(file.Title, file.Name));

            if (!string.IsNullOrEmpty(Page.Request.QueryString["x"]))
                _xsltDropDownList.Items.FindByValue(Page.Request.QueryString["x"]).Selected = true;
            else
                _xsltDropDownList.Items.FindByText("Default").Selected = true;
        }

        protected override void ConfigureDataSourceProperties()
        {
            if (_enableFql)
            {
                // We use the FixedQuery parameter to pass inn fql
                this.FixedQuery = GetQuery();
            }
            try
            {
                base.ConfigureDataSourceProperties();
            }
            catch (Exception x)
            {
                // MSFT base code does not handle null/empty queries gracefully.  We'll log it just in case some other issue cause the error
                Logger.Instance.Warning(string.Format("Exception caught in base.ConfigureDataSourceProperties() at {0}", (Page != null) ? Page.Request.Url.ToString() : "Unknown"), x, DiagnosticsCategories.eCaseSearch);
            }
        }

        protected override void CreateDataSource()
        {
            _query = HttpUtility.UrlDecode(HttpContext.Current.Request["k"]);
            _cacheKey = SPContext.Current.Site.Url;
            _synonymLookup = GetSynonymLookup(_cacheKey);
            if (IsSingleWordNoSynonyms())
            {
                // We can pass the query thru directly with no modifications
                // This will allow best bets to function
                _enableFql = false;
                this.FixedQuery = string.Empty;
            }
            else
            {
                _enableFql = true;
            }
            this.DataSource = new eCaseCoreResultsDataSource(this, _enableFql, _duplicateTrimProperty);
        }

        private bool IsSingleWordNoSynonyms()
        {
            return string.IsNullOrEmpty(_query) || _query == "#" || (!_reNonCharacter.IsMatch(_query) && !_synonymLookup.ContainsKey(_query.ToLower()));
        }

        private string GetQuery()
        {
            if (string.IsNullOrEmpty(_query))
            {
                return null;
            }
            Logger.Instance.Info(String.Format("GetQuery Called - Incoming Query: {0}", _query), DiagnosticsCategories.eCaseSearch);

            if (QueryKind == QueryKind.Fql && _enableFql) return _query;
            if (QueryKind == QueryKind.Kql && _query.ToLower().StartsWith("fql:"))
            {
                _query = _query.Substring(4);
                return _query;
            }

            return ConvertKqlToFql();
        }

        private string ConvertKqlToFql()
        {
            Logger.Instance.Info("Converting Kql to Fql", DiagnosticsCategories.eCaseSearch);
            Dictionary<string, string> scopeLookup = GetScopeLookup(_cacheKey);
            Dictionary<string, string> managedPropertyTypeLookup = GetPropertyTypeLookup(_cacheKey);

            string scopeFilter = null;
            if (!string.IsNullOrEmpty(this.Scope)) scopeLookup.TryGetValue(this.Scope.ToLower(), out scopeFilter);
            Logger.Instance.Info(String.Format("Requested Scope: {0} ; Scope Filter: {1}",
                                     this.Scope,
                                    String.IsNullOrEmpty(scopeFilter) ? "Not Found" : scopeFilter),
                                    DiagnosticsCategories.eCaseSearch);

            FqlHelper helper = new FqlHelper(_synonymLookup, managedPropertyTypeLookup, scopeFilter);
            var fql = helper.GetFqlFromKql(_query, SynonymHandling, BoostValue);

            Logger.Instance.Info(String.Format("Converting Kql to Fql Completed - Resulting FQL: {0}", fql), DiagnosticsCategories.eCaseSearch);

            return fql;
        }

        private Dictionary<string, string> GetPropertyTypeLookup(string uniqueKey)
        {
            Dictionary<string, string> propertyLookup;
            if (CacheMinutes == 0 || HttpContext.Current.Cache["props" + uniqueKey] == null)
            {
                propertyLookup = new Dictionary<string, string>();
                try { FastManagedPropertyReader.PopulateManagedProperties(propertyLookup); }
                catch (Exception x) { Logger.Instance.Error("Failed to get Managed Properties", x, DiagnosticsCategories.eCaseSearch); }
                HttpContext.Current.Cache.Add("scopes" + uniqueKey, propertyLookup, null, DateTime.UtcNow.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            else
            {
                propertyLookup = (Dictionary<string, string>)HttpContext.Current.Cache["props" + uniqueKey];
            }
            return propertyLookup;
        }

        private Dictionary<string, string> GetScopeLookup(string uniqueKey)
        {
            Dictionary<string, string> scopeLookup;
            if (CacheMinutes == 0 || HttpContext.Current.Cache["scopes" + uniqueKey] == null)
            {
                scopeLookup = new Dictionary<string, string>();
                try { FastScopeReader.PopulateScopes(scopeLookup); }
                catch (Exception x) { Logger.Instance.Error("Failed to Populate Search Scopes", x, DiagnosticsCategories.eCaseSearch); }
                HttpContext.Current.Cache.Add("scopes" + uniqueKey, scopeLookup, null, DateTime.UtcNow.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            else
            {
                scopeLookup = (Dictionary<string, string>)HttpContext.Current.Cache["scopes" + uniqueKey];
            }
            return scopeLookup;
        }

        private Dictionary<string, List<string>> GetSynonymLookup(string uniqueKey)
        {
            Dictionary<string, List<string>> synonymLookup;
            if (CacheMinutes == 0 || HttpContext.Current.Cache["synonyms" + uniqueKey] == null)
            {
                synonymLookup = new Dictionary<string, List<string>>();
                try { FastSynonymReader.PopulateSynonyms(synonymLookup); }
                catch (Exception x) { Logger.Instance.Error("Failed to Get Synonyms", x, DiagnosticsCategories.eCaseSearch); }
                HttpContext.Current.Cache.Add("synonyms" + uniqueKey, synonymLookup, null, DateTime.UtcNow.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            else
            {
                synonymLookup = (Dictionary<string, List<string>>)HttpContext.Current.Cache["synonyms" + uniqueKey];
            }
            return synonymLookup;
        }
    }
}
