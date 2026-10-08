using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Config.Attributes;
using Shoko.Abstractions.Config.Enums;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   Level 6, lifecycle hooks: the server runs New, Load, Validate and Save
///   itself. "Reset to defaults" fills "Created", opening the form fills
///   "Loaded", "invalid" in Validated Text or 13 in Count is refused on save,
///   and a save goes through the Save hook, which stamps "Saved". The default
///   save button is replaced by the section's own.
/// </summary>
[Display(Name = "GenUI Test 6: Lifecycle Hooks")]
[Section(DisplaySectionType.FieldSet, ShowSaveAction = true)]
[HideDefaultSaveAction]
public class LifecycleHooksConfiguration : IConfiguration
{
    #region Members

    /// <summary>When the New hook made the defaults.</summary>
    [Visibility(DisplayVisibility.ReadOnly)]
    public string Created { get; set; } = string.Empty;

    /// <summary>When the Load hook last ran.</summary>
    [Visibility(DisplayVisibility.ReadOnly)]
    public string Loaded { get; set; } = string.Empty;

    /// <summary>When the Save hook last saved.</summary>
    [Visibility(DisplayVisibility.ReadOnly)]
    public string Saved { get; set; } = string.Empty;

    /// <summary>Refused by the Validate hook when it reads "invalid".</summary>
    public string ValidatedText { get; set; } = string.Empty;

    /// <summary>Refused by the Validate hook when it is 13.</summary>
    [Range(0, 100)]
    public int Count { get; set; } = 4;

    #endregion

    #region Hooks

    /// <summary>Stamps the defaults with when they were made.</summary>
    [ConfigurationAction(ConfigurationActionType.New)]
    public ConfigurationActionResult OnNew(ConfigurationActionContext<LifecycleHooksConfiguration> context)
    {
        context.Configuration.Created = DateTime.Now.ToString("T");
        return new(context.Configuration);
    }

    /// <summary>Loads the saved copy and stamps it with when it was loaded.</summary>
    [ConfigurationAction(ConfigurationActionType.Load)]
    public static ConfigurationActionResult OnLoad(ConfigurationActionContext<LifecycleHooksConfiguration> context)
    {
        var saved = context.ConfigurationService.Load<LifecycleHooksConfiguration>(copy: true);
        saved.Loaded = DateTime.Now.ToString("T");
        return new(saved);
    }

    /// <summary>Refuses "invalid" and 13.</summary>
    [ConfigurationAction(ConfigurationActionType.Validate)]
    public static ConfigurationActionResult OnValidate(LifecycleHooksConfiguration configuration)
    {
        var errors = new Dictionary<string, IReadOnlyList<string>>();
        if (string.Equals(configuration.ValidatedText, "invalid", StringComparison.OrdinalIgnoreCase))
            errors[nameof(ValidatedText)] = ["The Validate hook refuses this value."];
        if (configuration.Count is 13)
            errors[nameof(Count)] = ["The Validate hook refuses 13."];
        return new() { ValidationErrors = errors };
    }

    /// <summary>Saves the document itself, stamped with when it was saved.</summary>
    [ConfigurationAction(ConfigurationActionType.Save)]
    public static ConfigurationActionResult OnSave(ConfigurationActionContext<LifecycleHooksConfiguration> context)
    {
        context.Configuration.Saved = DateTime.Now.ToString("T");
        context.ConfigurationService.Save(context.Configuration);
        return new(context.Configuration);
    }

    #endregion
}
