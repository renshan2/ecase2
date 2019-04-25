using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPFileExtensions
    {
        public static string GetContents(this SPFile file)
        {
            string content;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(file.OpenBinaryStream()))
            {
                content = reader.ReadToEnd();
            }
            return content;
        }

        public static bool Exists(this SPFile file, string destination)
        {
            using (SPSite site = new SPSite(destination))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPFile destinationFile = web.GetFile(destination + file.Name);
                    if (destinationFile.Exists)
                        return true;
                    else
                        return false;
                }
            }
        }
    }
}
