using MudBlazor;

namespace Shala.Web.Helpers;

public static class ShalaTheme
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#082A4D",
            PrimaryDarken = "#061F3A",
            PrimaryLighten = "#E6F1F8",
            Secondary = "#0FA3A0",
            SecondaryDarken = "#0B7F7C",
            Tertiary = "#FF8A1F",
            Info = "#2563EB",
            Success = "#0FA3A0",
            Warning = "#FF8A1F",
            Error = "#DC2626",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#082A4D",
            DrawerBackground = "#082A4D",
            DrawerText = "#FFFFFF",
            DrawerIcon = "#DDECF6",
            Background = "#F6F8FB",
            Surface = "#FFFFFF",
            TextPrimary = "#0B1F3A",
            TextSecondary = "#64748B",
            LinesDefault = "#E3EAF2",
            TableLines = "#E3EAF2",
            Divider = "#E3EAF2",
            ActionDefault = "#64748B",
            ActionDisabled = "#CBD5E1",
            ActionDisabledBackground = "#F1F5F9"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "sans-serif" },
                FontSize = ".925rem",
                FontWeight = "400",
                LineHeight = "1.5"
            },
            H1 = new H1Typography { FontSize = "2.5rem", FontWeight = "800", LineHeight = "1.12" },
            H2 = new H2Typography { FontSize = "2rem", FontWeight = "800", LineHeight = "1.16" },
            H3 = new H3Typography { FontSize = "1.65rem", FontWeight = "800", LineHeight = "1.2" },
            H4 = new H4Typography { FontSize = "1.4rem", FontWeight = "800", LineHeight = "1.25" },
            H5 = new H5Typography { FontSize = "1.15rem", FontWeight = "750", LineHeight = "1.3" },
            H6 = new H6Typography { FontSize = "1rem", FontWeight = "750", LineHeight = "1.35" },
            Button = new ButtonTypography { FontSize = ".875rem", FontWeight = "700", TextTransform = "none" },
            Caption = new CaptionTypography { FontSize = ".75rem", FontWeight = "500", LineHeight = "1.35" }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "14px",
            DrawerWidthLeft = "292px",
            AppbarHeight = "64px"
        }
    };
}
