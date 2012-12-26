namespace Nancy.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Nancy.Helpers;

    public static class ModuleExtensions
    {
        /// <summary>
        /// A regular expression used to manipulate parameterized route segments.
        /// </summary>
        /// <value>A <see cref="Regex"/> object.</value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Regex ModuleNameExpression =
            new Regex(@"(?<name>[\w]+)Module$", RegexOptions.Compiled);

        /// <summary>   
        /// Extracts the friendly name of a Nancy module given its type.
        /// </summary>
        /// <param name="name">The type name taken from GetType().Name.</param>
        /// <returns>A string containing the name of the parameter.</returns>
        /// <exception cref="FormatException"></exception>
        public static string GetModuleName(this NancyModule module)
        {
            var typeName = module.GetType().Name;
            var nameMatch =
                ModuleNameExpression.Match(typeName);

            if (nameMatch.Success)
            {
                return nameMatch.Groups["name"].Value;
            }

            return typeName;
        }


        public static string GetModulePath(this NancyModule module, NancyContext context)
        {
            if (string.IsNullOrEmpty(module.ModulePath))
            {
                return string.Empty;
            }
           
            if (!module.ModulePath.Contains("{")        // module path with parameters
                && !module.ModulePath.Contains("(?"))   // module path with regex
            {
                return module.ModulePath;
            }
            var modulePath = module.ModulePath.StartsWith("/") ? module.ModulePath : "/" + module.ModulePath;
            var segments = modulePath.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);

            var requestPath = context.Request.Path.StartsWith("/") ? context.Request.Path : "/" + context.Request.Path;
            var requestPathSegments = requestPath.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);

            if (requestPathSegments.Length<segments.Length)
            {
                return string.Empty;
            }
            
            return "/"+string.Join("/",requestPathSegments.Take(segments.Length));
        }
    }
}