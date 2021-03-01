using MCPMappingsLookup.Searching;
using TheRFramework.Utilities;

namespace MCPMappingsLookup.Views.Main
{
    public class MainViewModel : BaseViewModel
    {
        public SearchViewModel Search { get; set; }

        public MainViewModel()
        {
            Search = new SearchViewModel();
        }
    }
}
