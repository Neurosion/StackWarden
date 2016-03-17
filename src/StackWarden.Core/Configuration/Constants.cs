using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using StackWarden.Core.Extensions;

namespace StackWarden.Core.Configuration
{
    public static class Constants
    {
        private static readonly PluralizationService PluralizationService = PluralizationService.CreateService(CultureInfo.CurrentCulture);

        public static class Units
        {
            public const int Kibi = 1024;
        }

        public static class Categories
        {
            public const string Application = nameof(Application);
            public const string Service = nameof(Service);
            public const string Machine = nameof(Machine);
            public const string MessageQueue = nameof(MessageQueue);
            public const string Web = nameof(Web);
            public const string Database = nameof(Database);
            public const string Log = nameof(Log);
        }

        public static class Messages
        {
            public static class Arguments
            {
                public static string MustBeProvided(string argumentName)
                {
                    var expandedArgumentName = argumentName.ToExpandedString().ToLower();
                    var messagePrefix = expandedArgumentName.StartsWith("a")
                                            ? "An"
                                            : "A";

                    return $"{messagePrefix} {expandedArgumentName} must be provided.";
                }

                public static string AtLeastOne(string argumentName)
                {
                    var singularizedArgumentName = PluralizationService.Singularize(argumentName);
                    var expandedArgumentName = singularizedArgumentName.ToExpandedString().ToLower();
                                        
                    return $"At least one {expandedArgumentName} must be provided.";
                }
            }
        }
    }
}