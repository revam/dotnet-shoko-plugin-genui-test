using System.ComponentModel.DataAnnotations;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   Level 4, server options: members whose choices the server lists from the
///   current draft. Change the base port and reopen the port choices; they
///   should follow the unsaved value. Library choices arrive after a short wait,
///   with labels, and the same list serves two members.
/// </summary>
[Display(Name = "GenUI Test 4: Server Options")]
[Section(DisplaySectionType.FieldSet)]
public class ServerOptionsConfiguration : IConfiguration
{
    /// <summary>
    ///   Options the server lists on request.
    /// </summary>
    public OptionsSection Options { get; set; } = new();
}
