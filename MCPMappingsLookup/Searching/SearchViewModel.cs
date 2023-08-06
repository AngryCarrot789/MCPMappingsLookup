using MCPMappingsLookup.Variables;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Controls;
using TheRFramework.Utilities;

namespace MCPMappingsLookup.Searching
{
    public class SearchViewModel : BaseViewModel
    {
        private string _findInput;
        private MappingSearchType _findType;
        private bool _findFields;
        private bool _findFunctions;
        private string _results;
        private bool _findClasses;
        private bool _searchForExact;
        private bool _capsSensitive;
        private bool _searchAtIndex;

        public bool CanDisplayResults { get; set; }
        public Mappings Mappings { get; }

        public string Results
        {
            get => _results;
            set => RaisePropertyChanged(ref _results, value);
        }

        public string FindInput
        {
            get => _findInput;
            set => RaisePropertyChanged(ref _findInput, value, StartSearch);
        }

        public MappingSearchType FindType
        {
            get => _findType;
            set
            {
                if (value == MappingSearchType.Obfuscated)
                {
                    FindFields = false;
                    FindFunctions = false;
                    FindClasses = true;
                }
                else
                {
                    if (value == MappingSearchType.MCP)
                    {
                        if (_findType == MappingSearchType.Obfuscated)
                        {
                            FindFields = false;
                            FindFunctions = false;
                            FindClasses = true;
                        }
                        else
                        {
                            FindFields = true;
                            FindFunctions = true;
                            FindClasses = false;
                        }
                    }
                    else
                    {
                        FindFields = true;
                        FindFunctions = true;
                        FindClasses = false;
                    }
                }
                RaisePropertyChanged(ref _findType, value);
            }
        }

        public bool FindFields
        {
            get => _findFields;
            set => RaisePropertyChanged(ref _findFields, value);
        }

        public bool FindFunctions
        {
            get => _findFunctions;
            set => RaisePropertyChanged(ref _findFunctions, value);
        }

        public bool FindClasses
        {
            get => _findClasses;
            set => RaisePropertyChanged(ref _findClasses, value);
        }

        public bool SearchForExact
        {
            get => _searchForExact;
            set => RaisePropertyChanged(ref _searchForExact, value);
        }

        public bool SearchAtIndex
        {
            get => _searchAtIndex;
            set => RaisePropertyChanged(ref _searchAtIndex, value);
        }

        public bool CapsSensitive
        {
            get => _capsSensitive;
            set => RaisePropertyChanged(ref _capsSensitive, value);
        }

        public SearchViewModel()
        {
            string applicationPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            Mappings = new Mappings(
                Path.Combine(applicationPath, "fields.txt"), 
                Path.Combine(applicationPath, "functions.txt"),
                Path.Combine(applicationPath, "mcpToObfuscated.txt"));

            Results = "";
            FindType = MappingSearchType.MCP;
            FindFields = true;
            FindFunctions = true;
        }

        public void StartSearch(string text)
        {
            if (text == null || text == string.Empty)
            {
                return;
            }

            List<RemappedVariable> variables = new List<RemappedVariable>();

            if (FindFields)
            {
                variables.AddRange(Mappings.FindField(text, FindType, SearchForExact, SearchAtIndex, CapsSensitive));
            }
            if (FindFunctions)
            {
                variables.AddRange(Mappings.FindFunction(text, FindType, SearchForExact, SearchAtIndex, CapsSensitive));
            }
            if (FindClasses)
            {
                variables.AddRange(Mappings.FindClass(text, FindType, SearchForExact, SearchAtIndex, CapsSensitive));
            }

            //CanDisplayResults = true;
            DisplayResults(variables);
        }

        public void DisplayResults(List<RemappedVariable> variables)
        {
            StringBuilder text = new StringBuilder(Results.Length * 30);
            foreach(RemappedVariable variable in variables)
            {
                foreach(string remappedName in variable.RemappedNames)
                {
                    text.AppendLine($"({variable.OriginalName}): {remappedName}");
                }
            }

            Results = text.ToString();
        }
    }
}
