using System;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.KqlParser
{
    /// <summary>
    /// Specified the different kinds of kql tokens which are parsed
    /// This source code is released under the MIT license
    /// </summary>
    [Flags]
    public enum TokenType
    {
        Operator = 1, Phrase = 2, Group = 4, Word = 8, Property = 16
    }
}