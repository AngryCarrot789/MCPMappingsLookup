using MCPMappingsLookup.Themes;
using TheRFramework.Utilities;
using TheRThemes;

namespace MCPMappingsLookup.Views
{
    public class ThemesViewModel : BaseViewModel
    {
        public CommandParam<string> SetThemeCommand { get; }

        public ThemesViewModel()
        {
            SetThemeCommand = new CommandParam<string>(SetTheme);
        }

        public void SetTheme(string themeName)
        {
            switch (themeName)
            {
                case "nd":
                    ThemesController.SetTheme(ThemeType.Dark);
                    break;
                case "nl":
                    ThemesController.SetTheme(ThemeType.Light);
                    break;
                case "cd":
                    ThemesController.SetTheme(ThemeType.ColourfulDark);
                    break;
                case "cl":
                    ThemesController.SetTheme(ThemeType.ColourfulLight);
                    break;
            }
        }
    }
}
