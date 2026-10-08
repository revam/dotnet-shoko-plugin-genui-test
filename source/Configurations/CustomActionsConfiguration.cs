using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Config.Services;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   Level 3, custom actions: buttons the server runs on the current draft.
///   Each says what it does in its description: show a message, edit the draft,
///   refuse it with field errors, save and ask for a refresh, show the save
///   message or redirect.
/// </summary>
[Display(Name = "GenUI Test 3: Custom Actions")]
[Section(DisplaySectionType.FieldSet)]
public class CustomActionsConfiguration : IConfiguration
{
    /// <summary>
    ///   Buttons in every position and theme.
    /// </summary>
    public ActionsSection Actions { get; set; } = new();

    /// <summary>
    ///   Bumped in the saved copy by "Save And Refresh"; the form should reload
    ///   and show the new count.
    /// </summary>
    [Visibility(DisplayVisibility.ReadOnly)]
    public int SavedCounter { get; set; }

    /// <summary>
    ///   Refused by "Refuse" when it is empty.
    /// </summary>
    public string Required { get; set; } = string.Empty;

    /// <summary>
    ///   Puts a field error on <see cref="Required"/> when it is empty, and
    ///   leaves the draft alone otherwise.
    /// </summary>
    [Display(Name = "Refuse")]
    [CustomAction(Theme = DisplayColorTheme.Warning, Position = DisplayButtonPosition.End)]
    public ConfigurationActionResult RefuseAction()
        => string.IsNullOrWhiteSpace(Required)
            ? new() { ValidationErrors = new Dictionary<string, IReadOnlyList<string>> { [nameof(Required)] = ["Set by the \"Refuse\" action."] } }
            : new("Nothing to refuse.");

    /// <summary>
    ///   Bumps the counter in the saved copy, not in the draft, and asks the
    ///   client to reload the form, which should then show the new count.
    /// </summary>
    [Display(Name = "Save And Refresh")]
    [CustomAction(Theme = DisplayColorTheme.Primary, Position = DisplayButtonPosition.End)]
    public static ConfigurationActionResult SaveAndRefreshAction(IConfigurationService configurationService)
    {
        var saved = configurationService.Load<CustomActionsConfiguration>(copy: true);
        saved.SavedCounter++;
        configurationService.Save(saved);
        return new() { Refresh = true };
    }

    /// <summary>
    ///   Asks the client to show its usual "saved" message, saving nothing.
    /// </summary>
    [Display(Name = "Show Save Message")]
    [CustomAction(Position = DisplayButtonPosition.End)]
    public ConfigurationActionResult SaveMessageAction()
        => new() { ShowSaveMessage = true };

    /// <summary>
    ///   Redirects to the Shoko website, in a new tab.
    /// </summary>
    [Display(Name = "Redirect")]
    [CustomAction(Icon = "OpenInNew", Position = DisplayButtonPosition.End)]
    public ConfigurationActionResult RedirectAction()
        => new() { Redirect = new() { Location = "https://shokoanime.com" } };

    /// <summary>
    ///   Answers with two messages at once, one titled.
    /// </summary>
    [Display(Name = "Two Messages")]
    [CustomAction(Position = DisplayButtonPosition.End)]
    public ConfigurationActionResult MessagesAction()
        => new([
            new() { Title = "First", Message = $"Sent at {DateTime.Now:T}.", Theme = DisplayColorTheme.Primary },
            new() { Message = "A second message, untitled." },
        ]);
}
