using MCPMappingsLookup.Lists;
using MCPMappingsLookup.Variables;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Animation;
using TheRFramework.Utilities.String;

namespace MCPMappingsLookup.Searching
{
    public class Mappings
    {
        public const char Splitter1 = '\t';
        public const char Splitter2 = ' ';

        public MultiMap<string, string> MCPToObfuscatedClasses{ get; set; }
        public MultiMap<string, string> ObfuscatedToMCPClasses { get; set; }
        public MultiMap<string, string> MCPToSeargeFields { get; set; }
        public MultiMap<string, string> SeargeToMCPFields { get; set; }
        public MultiMap<string, string> MCPToSeargeFunctions { get; set; }
        public MultiMap<string, string> SeargeToMCPFunctions { get; set; }

        public Mappings(string fieldsPath, string functionsPath, string obfuscatedPath)
        {
            MCPToObfuscatedClasses = new MultiMap<string, string>();
            ObfuscatedToMCPClasses = new MultiMap<string, string>();
            MCPToSeargeFields = new MultiMap<string, string>();
            SeargeToMCPFields = new MultiMap<string, string>();
            MCPToSeargeFunctions = new MultiMap<string, string>();
            SeargeToMCPFunctions = new MultiMap<string, string>();
            LoadMappings(fieldsPath, functionsPath, obfuscatedPath);
        }

        public List<RemappedVariable> FindField(string searchValue, MappingSearchType searchType, bool searchExact = false, bool searchAtIndex = false, bool capsSensitive = false)
        {
            if (string.IsNullOrEmpty(searchValue))
                return null;

            List<RemappedVariable> fields = new List<RemappedVariable>(8000 / searchValue.Length);

            if (searchType == MappingSearchType.Searge)
                ExtractSearch(searchValue, fields, MCPToSeargeFields, searchExact, searchAtIndex, capsSensitive);

            else if (searchType == MappingSearchType.MCP)
                ExtractSearch(searchValue, fields, SeargeToMCPFields, searchExact, searchAtIndex, capsSensitive);

            return fields;
        }

        public List<RemappedVariable> FindFunction(string searchValue, MappingSearchType searchType, bool searchExact = false, bool searchAtIndex = false, bool capsSensitive = false)
        {
            if (string.IsNullOrEmpty(searchValue))
                return null;

            List<RemappedVariable> functions = new List<RemappedVariable>(8000 / searchValue.Length);

            if (searchType == MappingSearchType.Searge)
                ExtractSearch(searchValue, functions, MCPToSeargeFunctions, searchExact, searchAtIndex, capsSensitive);

            else if (searchType  == MappingSearchType.MCP)
                ExtractSearch(searchValue, functions, SeargeToMCPFunctions, searchExact, searchAtIndex, capsSensitive);

            return functions;
        }

        public List<RemappedVariable> FindClass(string searchValue, MappingSearchType searchType, bool searchExact = false, bool searchAtIndex = false, bool capsSensitive = false)
        {
            if (string.IsNullOrEmpty(searchValue))
                return null;

            List<RemappedVariable> classes = new List<RemappedVariable>(8000 / searchValue.Length);

            if (searchType == MappingSearchType.Obfuscated)
                ExtractSearch(searchValue, classes, MCPToObfuscatedClasses, searchExact, searchAtIndex, capsSensitive);

            else if (searchType == MappingSearchType.MCP)
                ExtractSearch(searchValue, classes, ObfuscatedToMCPClasses, searchExact, searchAtIndex, capsSensitive);

            return classes;
        }

        private void LoadMappings(string fieldsPath, string functionsPath, string obfuscatedPath)
        {
            if (fieldsPath != null && File.Exists(fieldsPath))
            {
                string[] contents = File.ReadAllLines(fieldsPath);
                foreach (string line in contents)
                {
                    int split = line.IndexOf(Splitter1);
                    if (split == -1)
                        split = line.IndexOf(Splitter2);
                    if (split == -1)
                        continue;

                    string searge = line.Substring(0, split).Trim();
                    string mcp = line.Substring(split + 1).Trim();
                    if (!searge.IsEmpty() || !mcp.IsEmpty())
                    {
                        PutField(mcp, searge);
                    }
                }
            }

            if (functionsPath != null && File.Exists(functionsPath))
            {
                string[] contents = File.ReadAllLines(functionsPath);
                foreach (string line in contents)
                {
                    int split = line.IndexOf(Splitter1);
                    if (split == -1)
                        split = line.IndexOf(Splitter2);
                    if (split == -1)
                        continue;

                    string searge = line.Substring(0, split).Trim();
                    string mcp = line.Substring(split + 1).Trim();
                    if (!searge.IsEmpty() || !mcp.IsEmpty())
                    {
                        PutFunction(mcp, searge);
                    }
                }
            }

            if (obfuscatedPath != null && File.Exists(obfuscatedPath))
            {
                string[] contents = File.ReadAllLines(obfuscatedPath);
                foreach (string line in contents)
                {
                    int split = line.IndexOf(Splitter2);
                    if (split == -1)
                        continue;

                    string mcp = line.Substring(0, split).Trim();
                    string obf = line.Substring(split + 1).Trim();
                    if (!mcp.IsEmpty() || !obf.IsEmpty())
                    {
                        PutClassObfuscated(mcp, obf);
                    }
                }
            }
        }

        private void PutField(string mcp, string searge)
        {
            MCPToSeargeFields.Add(mcp, searge);
            SeargeToMCPFields.Add(searge, mcp);
        }

        private void PutFunction(string mcp, string searge)
        {
            MCPToSeargeFunctions.Add(mcp, searge);
            SeargeToMCPFunctions.Add(searge, mcp);
        }

        private void PutClassObfuscated(string mcp, string obfuscated)
        {
            MCPToObfuscatedClasses.Add(mcp, obfuscated);
            ObfuscatedToMCPClasses.Add(obfuscated, mcp);
        }

        /// <summary>
        /// Tries to find the given <paramref name="rawSearchValue"/> within all of the keys within the
        /// given <paramref name="multiList"/> based on all of the given parameters for manipulating the search
        /// </summary>
        /// <param name="rawSearchValue">The raw value to be searched for (which may be edited based on parameters like <paramref name="searchAtIndex"/>)</param>
        /// <param name="results">A reference to a list where the results will be extracted too. couldve just returned a list... but eh</param>
        /// <param name="multiList">The multimap to search through</param>
        /// <param name="searchExact">Search for the exact string, of if <paramref name="searchAtIndex"/> is true, see if the text at the given index and length based on the trimmed <paramref name="rawSearchValue"/> value is equal</param>
        /// <param name="searchAtIndex">Searches for text at a given index (where whitespaces are used to determind the index increments. whitespaces only work at the start of the search value unfortunately...</param>
        /// <param name="capsSensitive">If the search should check if the cases are the same, or to ignore them (by making everything lowercase ;))</param>
        private static void ExtractSearch(
            string rawSearchValue,
            List<RemappedVariable> results,
            MultiMap<string, string> multiList,
            bool searchExact = false,
            bool searchAtIndex = false,
            bool capsSensitive = false)
        {
            string searchValue = capsSensitive ? rawSearchValue : rawSearchValue.ToLower();
            foreach (KeyValuePair<string, HashSet<string>> pair in multiList)
            {
                string pairKey = capsSensitive ? pair.Key : pair.Key.ToLower();

                if (pairKey.Length < searchValue.Length)
                    continue;

                if (searchAtIndex)
                {
                    string trimmedEnd = searchValue.TrimEnd();
                    int lastSpace = trimmedEnd.LastIndexOf(' ');
                    int wildcardCount = lastSpace + 1;
                    int startSearchIndex = wildcardCount;
                    string searchTrim = searchValue.TrimStart();

                    if (searchExact)
                    {
                        if (pairKey.IsIndexWithin(searchTrim.Length))
                        {
                            string region = pairKey.Substring(startSearchIndex, searchTrim.Length);
                            if (region.Contains(searchTrim))
                            {
                                results.Add(new RemappedVariable(pair));
                            }
                        }
                    }
                    else
                    {
                        if (pairKey.IsIndexWithin(startSearchIndex + searchTrim.Length))
                        {
                            string exactValue = pairKey.Substring(startSearchIndex);
                            if (exactValue == searchValue)
                            {
                                results.Add(new RemappedVariable(pair));
                            }
                        }
                    }
                }
                else
                {
                    if (searchExact)
                    {
                        if (pairKey == searchValue)
                        {
                            results.Add(new RemappedVariable(pair));
                        }
                    }
                    else
                    {
                        if (pairKey.Contains(searchValue))
                        {
                            results.Add(new RemappedVariable(pair));
                        }
                    }
                }
            }
        }
    }
}
