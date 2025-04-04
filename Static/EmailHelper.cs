namespace SlideCloud.Static
{
    public class EmailHelper
    {
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return email;

            var parts = email.Split('@');
            var username = parts[0];
            var domain = parts[1];

            if (username.Length <= 2)
                return "***@" + domain;

            var visible = username.Substring(0, 2);
            return visible + new string('*', username.Length - 2) + "@" + domain;
        }
    }
}
