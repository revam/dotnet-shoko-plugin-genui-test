using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   Level 1, save only: no hooks, no conditions, no live edit and no server
///   options. Every element and layout renders, nothing talks to the server
///   until Save, and after a save and a reload every value is as it was left.
/// </summary>
[Display(Name = "GenUI Test 1: Save Only")]
[Section(DisplaySectionType.Tab, DefaultSectionName = "General", AppendFloatingSectionsAtEnd = true)]
[FloatingSection("Login", Description = "A gathered section, described by the class.")]
public class SaveOnlyConfiguration : IConfiguration
{
    #region General

    /// <summary>
    ///   A loose member, gathered into the default section, with a badge.
    /// </summary>
    [Badge("Test", Theme = DisplayColorTheme.Primary)]
    public string Greeting { get; set; } = "Hello";

    /// <summary>
    ///   A member of a gathered section.
    /// </summary>
    [SectionName("Login")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    ///   A password in a gathered section.
    /// </summary>
    [SectionName("Login")]
    [PasswordPropertyText]
    public string Password { get; set; } = string.Empty;

    #endregion

    #region Tabs

    /// <summary>
    ///   Every scalar element.
    /// </summary>
    public PrimitivesSection Primitives { get; set; } = new();

    /// <summary>
    ///   Lists and dictionaries in every layout.
    /// </summary>
    public CollectionsSection Collections { get; set; } = new();

    /// <summary>
    ///   Selects whose options live in the value.
    /// </summary>
    public SelectsSection Selects { get; set; } = new();

    #endregion
}
