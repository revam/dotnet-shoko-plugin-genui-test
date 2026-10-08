using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Config.Attributes;
using Shoko.Abstractions.Config.Enums;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Components;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   Level 7, everything together, where the levels feed each other. Picking a
///   kind runs a live edit that sets the server-chosen mode, which a condition
///   reads to show the advanced member; the item choices follow the kind; and
///   "Flip" edits the draft so the same chain runs from a button.
/// </summary>
[Display(Name = "GenUI Test 7: Combined")]
[Section(DisplaySectionType.FieldSet)]
public class CombinedConfiguration : IConfiguration
{
    #region Members

    /// <summary>The kind; editing it runs the live edit.</summary>
    public string Kind { get; set; } = "fruit";

    /// <summary>Set by the live edit from <see cref="Kind"/>.</summary>
    [Visibility(DisplayVisibility.ReadOnly)]
    public bool IsAdvanced { get; set; }

    /// <summary>Shown only while <see cref="IsAdvanced"/> is on.</summary>
    [Visibility(DisplayVisibility.Hidden, ToggleWhenMemberIsSet = nameof(IsAdvanced), ToggleWhenSetTo = true, ToggleVisibilityTo = DisplayVisibility.Visible)]
    public string AdvancedNote { get; set; } = string.Empty;

    /// <summary>An item, from choices that follow <see cref="Kind"/>.</summary>
    public string Item { get; set; } = string.Empty;

    #endregion

    #region Behaviour

    /// <summary>Lists the choices for the drafted kind.</summary>
    [OptionsProvider(nameof(Item))]
    public SelectOption<string>[] ListItems()
        => Kind switch
        {
            "fruit" => [new("apple", "Apple"), new("pear", "Pear")],
            "tool" => [new("hammer", "Hammer"), new("saw", "Saw")],
            _ => [new("other", $"Something {Kind}")],
        };

    /// <summary>Lists the kinds.</summary>
    [OptionsProvider(nameof(Kind))]
    public static string[] ListKinds()
        => ["fruit", "tool", "advanced"];

    /// <summary>Turns on <see cref="IsAdvanced"/> for the advanced kind, and clears an item that no longer fits.</summary>
    [ConfigurationAction(ConfigurationActionType.LiveEdit, Events = [ReactiveEventType.Edited], ReactiveMembers = [nameof(Kind)])]
    public ConfigurationActionResult OnKindEdited(ConfigurationActionContext<CombinedConfiguration> context)
    {
        IsAdvanced = Kind is "advanced";
        if (!ListItems().Any(option => option.Value == Item))
            Item = string.Empty;
        return new(context.Configuration);
    }

    /// <summary>Switches between fruit and the advanced kind from a button.</summary>
    [Display(Name = "Flip")]
    [CustomAction(Theme = DisplayColorTheme.Primary)]
    public ConfigurationActionResult FlipAction(ConfigurationActionContext<CombinedConfiguration> context)
    {
        Kind = Kind is "advanced" ? "fruit" : "advanced";
        return OnKindEdited(context);
    }

    #endregion
}
