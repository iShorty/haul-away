
namespace CategorizedSearchBox
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public static class StringExtensions
    {
        // public static void Find

        public static bool Contains(this String str, String substring, StringComparison comp)
        {
            if (substring == null)
                throw new ArgumentNullException("substring",
                                             "substring cannot be null.");
            else if (!Enum.IsDefined(typeof(StringComparison), comp))
                throw new ArgumentException("comp is not a member of StringComparison",
                                         "comp");

            return str.IndexOf(substring, comp) >= 0;
        }

    }

}
