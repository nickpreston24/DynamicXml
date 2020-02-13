using System;
using System.Text.RegularExpressions;

namespace RegexBuilder
{
    public class RegexPatternAttribute : Attribute
    {
        string patternText;

        public RegexPatternAttribute(string patternText)
        {
            this.patternText = patternText;
            if (!IsValid())
                throw new Exception(nameof(patternText));
        }

        bool IsValid() => throw new NotImplementedException();
    }
}