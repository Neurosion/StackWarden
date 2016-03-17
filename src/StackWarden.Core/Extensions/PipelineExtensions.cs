using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace StackWarden.Core.Extensions
{
    public static class PipelineExtensions
    {
        public static IEnumerable<ErrorRecord> GetErrors(this Pipeline source)
        {
            var errors = source?.Error?.Read() as IEnumerable<ErrorRecord> 
                            ?? new ErrorRecord[0];

            return errors;
        }

        public static bool HasErrors(this Pipeline source)
        {
            var errorCount = source?.Error?.Count ?? 0;

            return errorCount > 0;
        }
    }
}