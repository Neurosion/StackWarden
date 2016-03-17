using System;
using System.Collections.Generic;
using System.Linq;

namespace StackWarden.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static TValue ThrowIf<TException, TValue>(this TValue source, bool conditionResult, params object[] exceptionArguments)
            where TException: Exception
        {
            if (!conditionResult)
                return source;

            var exceptionType = typeof(TException);
            var providedParameterTypes = exceptionArguments?.Select(x => x?.GetType())
                                                            .ToArray() 
                                            ?? new Type[0];
            var matchingConstructor = exceptionType.GetConstructor(providedParameterTypes);

            if (matchingConstructor == null)
            {
                var parameterTypeNames = string.Join(",", providedParameterTypes.Select(x => x.Name));
                throw new ArgumentException($"No constructor was found for '{exceptionType.Name}' with parameters ({parameterTypeNames})");
            }

            var exception = (TException)matchingConstructor.Invoke(exceptionArguments);

            throw exception;
        }

        public static T ThrowIfNull<T>(this T source, string argumentName)
            where T: class
        {
            return source.ThrowIf<ArgumentNullException, T>(source == null, argumentName);
        }

        public static string ThrowIfNullOrWhiteSpace(this string source, string argumentName, string message = null)
        {
            return source.ThrowIf<ArgumentException, string>(string.IsNullOrWhiteSpace(source), 
                                                             new[] 
                                                             {
                                                                 message ?? Configuration.Constants.Messages.Arguments.MustBeProvided(argumentName),
                                                                 argumentName
                                                             });
        }

        public static IEnumerable<T> ThrowIfNullOrEmpty<T>(this IEnumerable<T> source, string argumentName, string message = null)
        {
            return source.ThrowIf<ArgumentException, IEnumerable<T>>(source == null || !source.Any(), 
                                                                     new[] 
                                                                     {
                                                                         message ?? Configuration.Constants.Messages.Arguments.AtLeastOne(argumentName),
                                                                         argumentName
                                                                     });
        }

        public static IEnumerable<string> GetMessages(this Exception source)
        {
            for (var currentException = source; currentException != null; currentException = currentException.InnerException)
                yield return currentException.Message;
        }

        public static string GetMessages(this Exception source, string delimiter)
        {
            var messages = source.GetMessages();
            var joinedMessages = string.Join(delimiter, messages);

            return joinedMessages;
        }

        public static string ToDetailString(this Exception source)
        {
            var messages = source.GetMessages(" / ");
            var detail = $"{source.GetType().Name}: {messages}";

            return detail;
        }
    }
}