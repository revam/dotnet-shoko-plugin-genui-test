using System.ComponentModel.DataAnnotations;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   Level 2, client-side reactivity: conditions only. Changing the switch,
///   mode, level or text shows, hides, locks or disables the members below at
///   once, without a single request to the server.
/// </summary>
[Display(Name = "GenUI Test 2: Conditions")]
[Section(DisplaySectionType.FieldSet)]
public class ConditionsConfiguration : IConfiguration
{
    /// <summary>
    ///   Members shown, hidden or disabled by others, one per operator.
    /// </summary>
    public ConditionsSection Conditions { get; set; } = new();
}
