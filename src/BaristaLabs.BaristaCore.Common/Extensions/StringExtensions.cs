namespace BaristaLabs.BaristaCore.Extensions
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        /// <summary>
        /// By default, pascalize converts strings to UpperCamelCase also removing underscores
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Pascalize(this string input)
        {
            return Regex.Replace(input, "(?:^|_)(.)", match => match.Groups[1].Value.ToUpper());
        }

        /// <summary>
        /// Same as Pascalize except that the first character is lower case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Camelize(this string input)
        {
            var word = Pascalize(input);
            return word.Length > 0 ? word.Substring(0, 1).ToLower() + word.Substring(1) : word;
        }
    }
}
