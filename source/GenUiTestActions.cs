using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shoko.Abstractions.Actions;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Components;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   A global action whose parameters use most form features. Running it only
///   logs what it was given.
/// </summary>
/// <param name="logger">Logger.</param>
public sealed class GenUiTestGlobalAction(ILogger<GenUiTestGlobalAction> logger) : IExecutableAction
{
    #region Metadata

    /// <inheritdoc />
    public string Name => "GenUI Test: Global";

    /// <inheritdoc />
    public string? Description => "Logs its parameters. Tick \"Refuse\" to see validation turn it down.";

    /// <inheritdoc />
    public ActionPermission Permission => ActionPermission.User;

    /// <inheritdoc />
    public bool RequiresConfirmation => true;

    /// <inheritdoc />
    public string? ConfirmationMessage => "Run the GenUI test action? It only logs.";

    #endregion

    #region Parameters

    /// <summary>A required message.</summary>
    [Required]
    [MinLength(1)]
    public string Message { get; set; } = "Hello";

    /// <summary>How many times to log it.</summary>
    [Range(1, 5)]
    public int Repeat { get; set; } = 1;

    /// <summary>An enum parameter.</summary>
    public TestMode Mode { get; set; } = TestMode.Balanced;

    /// <summary>Tags, from labelled options.</summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>One tag, from the same options.</summary>
    public string? MainTag { get; set; }

    /// <summary>Whether validation should refuse the run.</summary>
    public bool Refuse { get; set; }

    /// <summary>Shown only while refusing.</summary>
    [Visibility(DisplayVisibility.Hidden, ToggleWhenMemberIsSet = nameof(Refuse), ToggleWhenSetTo = true, ToggleVisibilityTo = DisplayVisibility.Visible)]
    public string RefusalReason { get; set; } = "Refused on request.";

    /// <summary>Lists tags, labelled, offered for both tag parameters.</summary>
    [OptionsProvider(nameof(Tags), nameof(MainTag))]
    public SelectOption<string>[] ListTags()
        => [new("red", "Red"), new("green", "Green"), new($"mode-{Mode}".ToLowerInvariant(), $"Mode: {Mode}")];

    #endregion

    #region Execution

    /// <inheritdoc />
    public Task<ActionValidationResult?> Validate(CancellationToken token = default)
        => Task.FromResult(Refuse ? new ActionValidationResult(RefusalReason) : null);

    /// <inheritdoc />
    public Task Execute(CancellationToken token = default)
    {
        for (var index = 0; index < Repeat; index++)
            logger.LogInformation("GenUI test: {Message} ({Mode}; tags {Tags}; main {MainTag})", Message, Mode, string.Join(", ", Tags), MainTag);
        return Task.CompletedTask;
    }

    #endregion
}

/// <summary>
///   A series-scoped action whose options come from the series it runs on.
/// </summary>
/// <param name="logger">Logger.</param>
public sealed class GenUiTestSeriesAction(ILogger<GenUiTestSeriesAction> logger) : SeriesAction
{
    #region Metadata

    /// <inheritdoc />
    public override string Name => "GenUI Test: Series";

    /// <inheritdoc />
    public override string? Description => "Logs one of the series' titles, picked from options listed for that series.";

    /// <inheritdoc />
    public override ActionPermission Permission => ActionPermission.User;

    #endregion

    #region Parameters

    /// <summary>One of the series' titles.</summary>
    public string? Title { get; set; }

    /// <summary>Lists the titles of the series the action runs on.</summary>
    [OptionsProvider(nameof(Title))]
    public IReadOnlyList<SelectOption<string>> ListTitles()
        => [.. Series.Titles.Select(title => title.Value).Distinct().Select(title => new SelectOption<string>(title))];

    #endregion

    #region Execution

    /// <inheritdoc />
    public override Task Execute(CancellationToken token = default)
    {
        logger.LogInformation("GenUI test: series {SeriesID} picked {Title}.", Series.ID, Title ?? Series.Title);
        return Task.CompletedTask;
    }

    #endregion
}
