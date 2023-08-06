using System.Collections.Generic;

namespace MCPMappingsLookup.Variables
{
    /// <summary>
    /// Contains information on a named field or function, remapped to another or multiple alternative names.
    /// Technically a function isnt really a variable... i cant think of a better name though lol
    /// <para>
    ///     example 1, func_42069 is remapped to getStatistics
    /// </para>
    /// <para>
    ///     example 2, tileEntityX is remapped to field_133769 and field_69420_h (due to the tileEntityX name existing in different java classes)
    /// </para>
    /// </summary>
    public struct RemappedVariable
    {
        /// <summary>
        /// The original name for the field or function
        /// </summary>
        public string OriginalName { get; set; }
        /// <summary>
        /// The remapped names for the given field or function
        /// </summary>
        public HashSet<string> RemappedNames { get; set; }

        public RemappedVariable(KeyValuePair<string, HashSet<string>> pair)
        {
            OriginalName = pair.Key;
            RemappedNames = pair.Value;
        }
    }
}
