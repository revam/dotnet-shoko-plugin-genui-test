using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Config.Attributes;
using Shoko.Abstractions.Config.Enums;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   Level 5, live edit: handlers the server runs while the form is edited.
///   "Last Event" names every event that reached the server. Typing in Source
///   fills Mirror, focusing and leaving a watched field is reported, editing
///   the nested value runs its own handler first, adding a row prefills it, and
///   editing "Unwatched" sends nothing at all.
/// </summary>
[Display(Name = "GenUI Test 5: Live Edit")]
[Section(DisplaySectionType.FieldSet)]
public class LiveEditConfiguration : IConfiguration
{
    #region Members

    /// <summary>
    ///   The last event the server saw, with the path it came from.
    /// </summary>
    [Visibility(DisplayVisibility.ReadOnly)]
    public string LastEvent { get; set; } = "(nothing yet)";

    /// <summary>
    ///   Watched on edit: its reverse is written to <see cref="Mirror"/>.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    ///   Written by the edit handler.
    /// </summary>
    [Visibility(DisplayVisibility.ReadOnly)]
    public string Mirror { get; set; } = string.Empty;

    /// <summary>
    ///   Watched on focus and unfocus only.
    /// </summary>
    public string FocusMe { get; set; } = string.Empty;

    /// <summary>
    ///   Watched on click and view changes only.
    /// </summary>
    public bool ClickMe { get; set; }

    /// <summary>
    ///   Watched by nothing, so editing it sends no request.
    /// </summary>
    public string Unwatched { get; set; } = string.Empty;

    /// <summary>
    ///   A nested class with a handler of its own, run before the outer one.
    /// </summary>
    public LiveEditNested Nested { get; set; } = new();

    /// <summary>
    ///   Rows prefilled by a handler while one is being added.
    /// </summary>
    [List(ListType = DisplayListType.ComplexInline)]
    public List<LiveEditRow> Rows { get; set; } = [];

    #endregion

    #region Handlers

    /// <summary>
    ///   Mirrors <see cref="Source"/>, and reports edits to it and below
    ///   <see cref="Nested"/>.
    /// </summary>
    [ConfigurationAction(ConfigurationActionType.LiveEdit, Events = [ReactiveEventType.Edited], ReactiveMembers = [nameof(Source), nameof(Nested)])]
    public ConfigurationActionResult OnEdited(ConfigurationActionContext<LiveEditConfiguration> context)
    {
        Mirror = new string(Source.Reverse().ToArray());
        LastEvent = Describe(context);
        return new(context.Configuration);
    }

    /// <summary>
    ///   Reports focus changes on <see cref="FocusMe"/> and <see cref="Source"/>.
    /// </summary>
    [ConfigurationAction(ConfigurationActionType.LiveEdit, Events = [ReactiveEventType.Focused, ReactiveEventType.Unfocused], ReactiveMembers = [nameof(FocusMe), nameof(Source)])]
    public ConfigurationActionResult OnFocusChanged(ConfigurationActionContext<LiveEditConfiguration> context)
    {
        LastEvent = Describe(context);
        return new(context.Configuration);
    }

    /// <summary>
    ///   Reports clicks and view changes on <see cref="ClickMe"/>.
    /// </summary>
    [ConfigurationAction(ConfigurationActionType.LiveEdit, Events = [ReactiveEventType.Clicked, ReactiveEventType.ViewChanged], ReactiveMembers = [nameof(ClickMe)])]
    public ConfigurationActionResult OnClicked(ConfigurationActionContext<LiveEditConfiguration> context)
    {
        LastEvent = Describe(context);
        return new(context.Configuration);
    }

    private static string Describe(ConfigurationActionContext<LiveEditConfiguration> context)
        => $"{context.ReactiveEventType} at \"{context.Path}\" ({DateTime.Now:T})";

    #endregion
}

/// <summary>
///   A nested class reacting to its own edits.
/// </summary>
[Display(Name = "Nested")]
[Section(DisplaySectionType.FieldSet)]
public class LiveEditNested
{
    /// <summary>
    ///   Upper-cased into <see cref="Echo"/> by the nested handler.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    ///   Written by the nested handler.
    /// </summary>
    [Visibility(DisplayVisibility.ReadOnly)]
    public string Echo { get; set; } = string.Empty;

    /// <summary>
    ///   Upper-cases <see cref="Value"/>.
    /// </summary>
    // Names its member, though it could watch everything: SHOKO0007's check
    // throws on `Events` without `ReactiveMembers` in the current analyzer.
    [ConfigurationAction(ConfigurationActionType.LiveEdit, Events = [ReactiveEventType.Edited], ReactiveMembers = [nameof(Value)])]
    public ConfigurationActionResult OnEdited(ConfigurationActionContext<LiveEditConfiguration> context)
    {
        Echo = Value.ToUpperInvariant();
        return new(context.Configuration);
    }
}

/// <summary>
///   A row prefilled while it is being added.
/// </summary>
[Section(DisplaySectionType.FieldSet)]
public class LiveEditRow
{
    /// <summary>The row's key.</summary>
    [Key]
    public string ID { get; set; } = string.Empty;

    /// <summary>Prefilled when the row is added.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    ///   Fills in the key and label of a row being added.
    /// </summary>
    // Names its members for the same analyzer bug as the nested handler.
    [ConfigurationAction(ConfigurationActionType.LiveEdit, Events = [ReactiveEventType.NewValue], ReactiveMembers = [nameof(ID), nameof(Label)])]
    public ConfigurationActionResult OnNewValue(ConfigurationActionContext<LiveEditConfiguration> context)
    {
        if (string.IsNullOrEmpty(ID))
            ID = Guid.NewGuid().ToString("N")[..8];
        if (string.IsNullOrEmpty(Label))
            Label = $"Added at {DateTime.Now:T}";
        return new(context.Configuration);
    }
}
