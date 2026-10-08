using System.ComponentModel.DataAnnotations;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   Level 4, server options: members whose choices the server lists from the
///   current draft. Change the base port and reopen the port choices; they
///   should follow the unsaved value. Library choices arrive after a short wait,
///   with labels, and the same list serves two members. A dictionary's names
///   and ports come from providers of their own, the entries of a dictionary's
///   lists take options, the flags offer Red and Blue only, a plugin colour
///   type is labelled by its converter, a plugin version type
///   parsable from text renders as text, and
///   the remote library refuses the draft with a field error on the API key
///   until one is set.
/// </summary>
[Display(Name = "GenUI Test 4: Server Options")]
[Section(DisplaySectionType.FieldSet)]
public class ServerOptionsConfiguration : INewtonsoftJsonConfiguration
{
    /// <summary>
    ///   Options the server lists on request.
    /// </summary>
    public OptionsSection Options { get; set; } = new();

    /// <summary>
    ///   Dictionaries keyed by Guid, with labelled keys, values or both.
    /// </summary>
    public KeyedByIdSection KeyedByID { get; set; } = new();
}
