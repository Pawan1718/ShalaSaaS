using MudBlazor;

namespace Shala.Web.Components.Common;

public sealed class AppSectionSidebarItem
{
    public string Key { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Icon { get; set; } = Icons.Material.Filled.Settings;

    public string? Badge { get; set; }
    public Color BadgeColor { get; set; } = Color.Primary;

    public bool Disabled { get; set; }
}