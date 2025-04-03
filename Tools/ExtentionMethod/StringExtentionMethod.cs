
    public static class StringExtentionMethod
    {
        public static string GetTextBy20Length(this string text) {

            return text?.Length > 20
                ? text?.Substring(0, 20) + "..."
                : text;

            
        }
    }

