using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Components;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   Dictionaries keyed by Guid, with labelled options for their keys, their
///   values or both. Known keys should show their plugin's name, and a key
///   no provider lists its raw Guid.
/// </summary>
[Display(Name = "Keyed by ID")]
[Section(DisplaySectionType.FieldSet)]
public class KeyedByIdSection
{
    #region Fixed IDs

    private static readonly Guid _alpha = new("0b7e6a52-3d1f-4c69-9d0a-3f0f3d2a1a01");

    private static readonly Guid _beta = new("0b7e6a52-3d1f-4c69-9d0a-3f0f3d2a1a02");

    private static readonly Guid _gamma = new("0b7e6a52-3d1f-4c69-9d0a-3f0f3d2a1a03");

    private static readonly Guid _unknown = new("0b7e6a52-3d1f-4c69-9d0a-3f0f3d2a1aff");

    #endregion

    #region Members

    /// <summary>
    ///   Keys picked from labelled plugins, values typed freely. The key picker
    ///   and the existing "Alpha" entry should show plugin names, not Guids.
    /// </summary>
    public Dictionary<Guid, string> NotesByPlugin { get; set; } = new() { [_alpha] = "First note" };

    /// <summary>
    ///   Keys typed freely as Guids, values picked from labelled priorities. The
    ///   value of the existing entry should show "Normal", not 50.
    /// </summary>
    public Dictionary<Guid, int> PriorityByID { get; set; } = new() { [_beta] = 50 };

    /// <summary>
    ///   Keys and values each from a provider of their own, both labelled. The
    ///   existing entry should read "Gamma" with the mode "Fast lane".
    /// </summary>
    public Dictionary<Guid, string> ModeByPlugin { get; set; } = new() { [_gamma] = "fast" };

    /// <summary>
    ///   Shares its key provider with <see cref="NotesByPlugin"/>; the same
    ///   plugin names should be offered.
    /// </summary>
    public Dictionary<Guid, bool> EnabledByPlugin { get; set; } = new() { [_beta] = true };

    /// <summary>
    ///   Holds a key no provider lists next to a known one. "Alpha" should show
    ///   its name, and the other entry its raw Guid
    ///   (0b7e6a52-3d1f-4c69-9d0a-3f0f3d2a1aff).
    /// </summary>
    public Dictionary<Guid, string> WithUnknownKey { get; set; } = new() { [_alpha] = "known", [_unknown] = "unknown" };

    #endregion

    #region Providers

    /// <summary>Lists the fake plugins, for the keys of every plugin-keyed dictionary.</summary>
    [OptionsProvider(nameof(NotesByPlugin), nameof(EnabledByPlugin), nameof(ModeByPlugin), nameof(WithUnknownKey), Target = OptionsTarget.Keys)]
    public static SelectOption<Guid>[] ListPlugins()
        => [new(_alpha, "Alpha"), new(_beta, "Beta"), new(_gamma, "Gamma")];

    /// <summary>Lists labelled priorities.</summary>
    [OptionsProvider(nameof(PriorityByID))]
    public static SelectOption<int>[] ListPriorities()
        => [new(10, "Low"), new(50, "Normal"), new(90, "High")];

    /// <summary>Lists labelled modes.</summary>
    [OptionsProvider(nameof(ModeByPlugin))]
    public static SelectOption<string>[] ListModes()
        => [new("slow", "Slow and steady"), new("fast", "Fast lane")];

    #endregion
}
