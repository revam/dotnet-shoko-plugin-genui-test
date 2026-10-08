using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shoko.Abstractions.Config;
using Shoko.Abstractions.Config.Attributes;
using Shoko.Abstractions.Config.Enums;
using Shoko.Abstractions.UI.Attributes;
using Shoko.Abstractions.UI.Components;
using Shoko.Abstractions.Exceptions;
using Shoko.Abstractions.UI.Enums;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   An enum, with a description on each member.
/// </summary>
public enum TestMode
{
    /// <summary>The first mode.</summary>
    [Description("Takes its time.")]
    Slow = 0,

    /// <summary>The second mode.</summary>
    [Description("Somewhere in between.")]
    Balanced = 1,

    /// <summary>The third mode.</summary>
    [Description("Goes as fast as it can.")]
    Fast = 2,
}

/// <summary>
///   A flags enum.
/// </summary>
[Flags]
public enum TestFlags
{
    /// <summary>Nothing.</summary>
    None = 0,

    /// <summary>The first flag.</summary>
    Red = 1,

    /// <summary>The second flag.</summary>
    Green = 2,

    /// <summary>The third flag.</summary>
    Blue = 4,
}

/// <summary>
///   Every scalar element, with constraints.
/// </summary>
[Display(Name = "Primitives")]
[Section(DisplaySectionType.FieldSet)]
public class PrimitivesSection
{
    /// <summary>A string with a default and length limits.</summary>
    [DefaultValue("shoko")]
    [StringLength(20, MinimumLength = 2)]
    public string Name { get; set; } = "shoko";

    /// <summary>A string matching a pattern.</summary>
    [RegularExpression("^[a-z]+$")]
    public string LowercaseOnly { get; set; } = "abc";

    /// <summary>A URL.</summary>
    [Url]
    public string Website { get; set; } = "https://shokoanime.com";

    /// <summary>A boolean that needs a restart and has an environment variable.</summary>
    [RequiresRestart]
    [EnvironmentVariable("GENUI_TEST_ENABLED")]
    public bool Enabled { get; set; } = true;

    /// <summary>An integer in a range, drawn small.</summary>
    [Range(0, 100)]
    [Visibility(Size = DisplayElementSize.Small)]
    public int Count { get; set; } = 4;

    /// <summary>A nullable integer.</summary>
    public int? Optional { get; set; }

    /// <summary>A float with denied values.</summary>
    [DeniedValues(0.0, 1.0)]
    public double Ratio { get; set; } = 0.5;

    /// <summary>An enum.</summary>
    public TestMode Mode { get; set; } = TestMode.Balanced;

    /// <summary>A flags enum.</summary>
    public TestFlags Flags { get; set; } = TestFlags.Red;

    /// <summary>A password, through the data type.</summary>
    [DataType(DataType.Password)]
    public string Token { get; set; } = string.Empty;

    /// <summary>A text area.</summary>
    [TextArea]
    [Visibility(Size = DisplayElementSize.Full)]
    public string Notes { get; set; } = string.Empty;

    /// <summary>A JSON code editor, formatted on load.</summary>
    [CodeEditor(CodeEditorLanguage.Json, AutoFormatOnLoad = true)]
    [Visibility(Size = DisplayElementSize.Full)]
    public string Json { get; set; } = "{\"key\":\"value\"}";

    /// <summary>A member only shown among the advanced ones.</summary>
    [Visibility(Advanced = true)]
    public string AdvancedText { get; set; } = string.Empty;

    /// <summary>A read-only member.</summary>
    [Visibility(DisplayVisibility.ReadOnly)]
    public string ReadOnlyText { get; set; } = "You cannot edit this.";

    /// <summary>A hidden member, which should not render.</summary>
    [Visibility(DisplayVisibility.Hidden)]
    public string HiddenText { get; set; } = "You should not see this.";
}

/// <summary>
///   An entry of a complex list, keyed and titled.
/// </summary>
[Section(DisplaySectionType.FieldSet)]
public class TestRow
{
    /// <summary>The entry's key.</summary>
    [Key]
    public string ID { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>The entry's title, shown as its label.</summary>
    public TitleComponent Title { get; set; } = new() { Title = "Row", SubTitle = "A complex entry" };

    /// <summary>A value of the entry.</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>A mode of the entry.</summary>
    public TestMode Mode { get; set; } = TestMode.Slow;
}

/// <summary>
///   Lists and dictionaries.
/// </summary>
[Display(Name = "Collections")]
[Section(DisplaySectionType.Minimal)]
public class CollectionsSection
{
    /// <summary>A flat list of strings, unique and sortable.</summary>
    [List(UniqueItems = true, Sortable = true)]
    public List<string> Strings { get; set; } = ["one", "two"];

    /// <summary>A list of numbers that cannot be removed from.</summary>
    [List(HideRemoveAction = true)]
    public List<int> Numbers { get; set; } = [1, 2, 3];

    /// <summary>A list of enum values drawn as checkboxes.</summary>
    [List(ListType = DisplayListType.EnumCheckbox)]
    public List<TestMode> Modes { get; set; } = [TestMode.Fast];

    /// <summary>A complex list drawn as tabs.</summary>
    [List(ListType = DisplayListType.ComplexTab)]
    public List<TestRow> TabRows { get; set; } = [new()];

    /// <summary>A complex list drawn as a dropdown.</summary>
    [List(ListType = DisplayListType.ComplexDropdown)]
    public List<TestRow> DropdownRows { get; set; } = [new()];

    /// <summary>A complex list drawn inline.</summary>
    [List(ListType = DisplayListType.ComplexInline)]
    public List<TestRow> InlineRows { get; set; } = [new()];

    /// <summary>A dictionary of booleans by name.</summary>
    public Dictionary<string, bool> Toggles { get; set; } = new() { ["first"] = true, ["second"] = false };

    /// <summary>A dictionary keyed by an enum, without a remove button.</summary>
    [Record(HideRemoveAction = true)]
    public Dictionary<TestMode, int> Weights { get; set; } = new() { [TestMode.Slow] = 1 };

    /// <summary>A dictionary of complex values drawn as tabs.</summary>
    [Record(RecordType = DisplayRecordType.ComplexTab)]
    public Dictionary<string, TestRow> NamedRows { get; set; } = new() { ["main"] = new() };

    /// <summary>A dictionary of lists.</summary>
    public Dictionary<string, List<string>> Groups { get; set; } = new() { ["letters"] = ["a", "b"] };
}

/// <summary>
///   Members shown, hidden or disabled by other members, one per operator.
/// </summary>
[Display(Name = "Conditions")]
[Section(DisplaySectionType.FieldSet)]
public class ConditionsSection
{
    /// <summary>The switch most conditions below look at.</summary>
    public bool Enabled { get; set; }

    /// <summary>A mode some conditions look at.</summary>
    public TestMode Mode { get; set; } = TestMode.Slow;

    /// <summary>A number some conditions look at.</summary>
    [Range(0, 10)]
    public int Level { get; set; } = 5;

    /// <summary>A text some conditions look at.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Shown while <see cref="Enabled"/> is on.</summary>
    [Visibility(DisplayVisibility.Hidden, ToggleWhenMemberIsSet = nameof(Enabled), ToggleWhenSetTo = true, ToggleVisibilityTo = DisplayVisibility.Visible)]
    public string ShownWhenEnabled { get; set; } = string.Empty;

    /// <summary>Read-only while <see cref="Enabled"/> is off.</summary>
    [Visibility(ToggleWhenMemberIsSet = nameof(Enabled), ToggleWhenSetTo = false, ToggleVisibilityTo = DisplayVisibility.ReadOnly)]
    public string ReadOnlyWhenDisabled { get; set; } = string.Empty;

    /// <summary>Disabled unless the mode is fast.</summary>
    [Visibility(DisableWhenMemberIsSet = nameof(Mode), DisableOperator = UiConditionOperator.NotEquals, DisableWhenSetTo = TestMode.Fast)]
    public string EnabledOnlyWhenFast { get; set; } = string.Empty;

    /// <summary>Hidden while <see cref="Text"/> is empty.</summary>
    [Visibility(ToggleWhenMemberIsSet = nameof(Text), ToggleOperator = UiConditionOperator.IsEmpty, ToggleVisibilityTo = DisplayVisibility.Hidden)]
    public string ShownWhenTextIsSet { get; set; } = string.Empty;

    /// <summary>Hidden while <see cref="Text"/> is set.</summary>
    [Visibility(ToggleWhenMemberIsSet = nameof(Text), ToggleOperator = UiConditionOperator.IsNotEmpty, ToggleVisibilityTo = DisplayVisibility.Hidden)]
    public string ShownWhenTextIsEmpty { get; set; } = string.Empty;

    /// <summary>Hidden while the mode is slow or balanced.</summary>
    [Visibility(ToggleWhenMemberIsSet = nameof(Mode), ToggleOperator = UiConditionOperator.In, ToggleWhenSetToAny = [TestMode.Slow, TestMode.Balanced], ToggleVisibilityTo = DisplayVisibility.Hidden)]
    public string ShownWhenFast { get; set; } = string.Empty;

    /// <summary>Hidden unless the mode is slow or balanced.</summary>
    [Visibility(ToggleWhenMemberIsSet = nameof(Mode), ToggleOperator = UiConditionOperator.NotIn, ToggleWhenSetToAny = [TestMode.Slow, TestMode.Balanced], ToggleVisibilityTo = DisplayVisibility.Hidden)]
    public string ShownWhenNotFast { get; set; } = string.Empty;

    /// <summary>Hidden while the level is above 7.</summary>
    [Visibility(ToggleWhenMemberIsSet = nameof(Level), ToggleOperator = UiConditionOperator.GreaterThan, ToggleWhenSetTo = 7, ToggleVisibilityTo = DisplayVisibility.Hidden)]
    public string ShownAtLowLevels { get; set; } = string.Empty;

    /// <summary>Hidden while the level is below 3.</summary>
    [Visibility(ToggleWhenMemberIsSet = nameof(Level), ToggleOperator = UiConditionOperator.LessThan, ToggleWhenSetTo = 3, ToggleVisibilityTo = DisplayVisibility.Hidden)]
    public string ShownAtHighLevels { get; set; } = string.Empty;

    /// <summary>Hidden while <see cref="Text"/> contains "hide".</summary>
    [Visibility(ToggleWhenMemberIsSet = nameof(Text), ToggleOperator = UiConditionOperator.Contains, ToggleWhenSetTo = "hide", ToggleVisibilityTo = DisplayVisibility.Hidden)]
    public string HiddenByContains { get; set; } = string.Empty;

    /// <summary>A section drawn as a checkbox.</summary>
    public CheckboxSection Experimental { get; set; } = new();
}

/// <summary>
///   A nested section drawn as a checkbox.
/// </summary>
[Display(Name = "Experimental")]
[Section(DisplaySectionType.Checkbox)]
public class CheckboxSection
{
    /// <summary>Whether the section is on.</summary>
    public bool Enabled { get; set; }

    /// <summary>A member of the section.</summary>
    public string Value { get; set; } = string.Empty;
}

/// <summary>
///   An entry whose options depend on its own values.
/// </summary>
[Section(DisplaySectionType.FieldSet)]
public class OptionsRow
{
    /// <summary>The entry's key.</summary>
    [Key]
    public string ID { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>A prefix the options below are built from.</summary>
    public string Prefix { get; set; } = "item";

    /// <summary>A choice from options built out of <see cref="Prefix"/>.</summary>
    public string Choice { get; set; } = string.Empty;

    /// <summary>Lists choices from the entry's own prefix.</summary>
    [OptionsProvider(nameof(Choice))]
    public IEnumerable<string> ListChoices()
        => Enumerable.Range(1, 3).Select(index => $"{Prefix}-{index}");
}

/// <summary>
///   Options the server lists on request.
/// </summary>
[Display(Name = "Options")]
[Section(DisplaySectionType.FieldSet)]
public class OptionsSection
{
    /// <summary>The base the port options are counted from.</summary>
    [Range(1, 65000)]
    public int BasePort { get; set; } = 8000;

    /// <summary>A port, from options counted from <see cref="BasePort"/>, unsaved edits included.</summary>
    public int Port { get; set; } = 8000;

    /// <summary>A library, from labelled options shared with <see cref="ExtraLibraries"/>.</summary>
    public int? Library { get; set; }

    /// <summary>More libraries, from the same labelled options.</summary>
    public List<int> ExtraLibraries { get; set; } = [];

    /// <summary>A colour, from a static provider.</summary>
    public string Colour { get; set; } = "red";

    /// <summary>A mode, from a subset of the enum.</summary>
    public TestMode Mode { get; set; } = TestMode.Slow;

    /// <summary>Entries with options of their own.</summary>
    [List(ListType = DisplayListType.ComplexInline)]
    public List<OptionsRow> Rows { get; set; } = [new()];

    /// <summary>Ports by name: the names and the ports each come from a provider of their own.</summary>
    public Dictionary<string, int> PortsByName { get; set; } = new() { ["web"] = 8000 };

    /// <summary>Tags by mode, a dictionary of lists whose entries take options.</summary>
    public Dictionary<TestMode, List<string>> TagsByMode { get; set; } = [];

    /// <summary>A plugin type converted to and from text, labelled by its converter.</summary>
    public TestTint Tint { get; set; } = new(0, 0, 0);

    /// <summary>A plugin type parsable from text, without a converter.</summary>
    public TestVersion Version { get; set; } = new(1, 0);

    /// <summary>Needed by the library listing below, which refuses the draft without it.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>A remote library, listed only once <see cref="ApiKey"/> is set.</summary>
    public string RemoteLibrary { get; set; } = string.Empty;

    /// <summary>Lists ports counted from the edited base, for the port and the dictionary's values.</summary>
    [OptionsProvider(nameof(Port), nameof(PortsByName))]
    public int[] ListPorts()
        => [BasePort, BasePort + 1, BasePort + 2];

    /// <summary>Lists names for the dictionary's keys.</summary>
    [OptionsProvider(nameof(PortsByName), Target = OptionsTarget.Keys)]
    public static string[] ListPortNames()
        => ["web", "api", "metrics"];

    /// <summary>Lists tags for the entries of the dictionary's lists.</summary>
    [OptionsProvider(nameof(TagsByMode))]
    public static SelectOption<string>[] ListTags()
        => [new("new", "New"), new("hot", "Hot"), new("old")];

    /// <summary>Lists tints; their labels come from the converter.</summary>
    [OptionsProvider(nameof(Tint))]
    public static TestTint[] ListTints()
        => [new(255, 0, 0), new(0, 128, 0), new(0, 0, 255)];

    /// <summary>Lists versions.</summary>
    [OptionsProvider(nameof(Version))]
    public static IEnumerable<TestVersion> ListVersions()
        => [new(1, 0), new(1, 1), new(2, 0)];

    /// <summary>Refuses the draft without an API key, as a field error on it.</summary>
    [OptionsProvider(nameof(RemoteLibrary))]
    public string[] ListRemoteLibraries()
        => string.IsNullOrWhiteSpace(ApiKey)
            ? throw new GenericValidationException(
                "No API key.",
                new Dictionary<string, IReadOnlyList<string>> { [$"{nameof(ServerOptionsConfiguration.Options)}.{nameof(ApiKey)}"] = ["Set an API key to list the remote libraries."] }
            )
            : ["Remote Anime", "Remote Movies"];

    /// <summary>Lists libraries with labels, after a short wait.</summary>
    [OptionsProvider(nameof(Library), nameof(ExtraLibraries))]
    public async Task<IReadOnlyList<SelectOption<int>>> ListLibraries()
    {
        await Task.Delay(100);
        return [new(1, "Anime"), new(2, "Movies"), new(3, "Music Videos")];
    }

    /// <summary>Lists colours.</summary>
    [OptionsProvider(nameof(Colour))]
    public static IReadOnlyList<SelectOption<string>> ListColours()
        => [new("red", "Red"), new("green", "Green"), new("blue")];

    /// <summary>Lists two of the three modes.</summary>
    [OptionsProvider(nameof(Mode))]
    public static TestMode[] ListModes()
        => [TestMode.Slow, TestMode.Fast];
}

/// <summary>
///   Selects whose options live in the value.
/// </summary>
[Display(Name = "Selects")]
[Section(DisplaySectionType.FieldSet)]
public class SelectsSection
{
    /// <summary>A single choice, drawn as a flat list.</summary>
    [Select(DisplaySelectType.FlatList)]
    public SelectComponent<string> Single { get; set; } = new([new("a", "Alpha", isSelected: true), new("b", "Beta"), new("c", "Gamma", isDisabled: true)]);

    /// <summary>Several choices, drawn as checkboxes.</summary>
    [Select(DisplaySelectType.CheckboxList, MultipleItems = true)]
    public SelectComponent<int> Multiple { get; set; } = new([new(1, "One", isDefault: true), new(2, "Two"), new(3, "Three")]);

    /// <summary>Choices in groups, rendered as the client sees fit.</summary>
    public SelectComponent<string> Grouped { get; set; } = new(
        [new("x", "Ex") { GroupID = 1 }, new("y", "Why") { GroupID = 1 }, new("z", "Zed") { GroupID = 2 }],
        [new() { ID = 1, Label = "First" }, new() { ID = 2, Label = "Second" }]
    );
}

/// <summary>
///   Buttons in every position and theme.
/// </summary>
[Display(Name = "Actions")]
[Section(DisplaySectionType.FieldSet)]
[FloatingSection("Danger", Description = "Buttons gathered into a section of their own.")]
public class ActionsSection
{
    /// <summary>A switch the buttons below look at.</summary>
    public bool Armed { get; set; }

    /// <summary>Pinned to the top.</summary>
    [Display(Name = "Say Hello")]
    [CustomAction(Icon = "HandWave", Theme = DisplayColorTheme.Primary, Position = DisplayButtonPosition.Start)]
    public ConfigurationActionResult HelloAction(ILogger logger)
    {
        logger.LogInformation("GenUI test: hello.");
        return new("Hello from the server.", DisplayColorTheme.Primary);
    }

    /// <summary>Inline, among the fields.</summary>
    [Display(Name = "Inline")]
    [CustomAction(Size = DisplayElementSize.Small)]
    public ConfigurationActionResult InlineAction()
        => new("An inline action ran.");

    /// <summary>Pinned to the bottom, disabled until something changed.</summary>
    [Display(Name = "Only With Changes")]
    [CustomAction(Theme = DisplayColorTheme.Secondary, Position = DisplayButtonPosition.End, DisableIfNoChanges = true)]
    public ConfigurationActionResult ChangedAction()
        => new("Something changed.", DisplayColorTheme.Secondary);

    /// <summary>Shown only while armed, in a gathered section.</summary>
    [Display(Name = "Fire")]
    [CustomAction(Theme = DisplayColorTheme.Danger, SectionName = "Danger", ToggleWhenMemberIsSet = nameof(Armed), ToggleWhenSetTo = true)]
    public ConfigurationActionResult FireAction()
        => new("Fired. Nothing happened.", DisplayColorTheme.Danger);

    /// <summary>Disabled while armed, in a gathered section.</summary>
    [Display(Name = "Safe Option")]
    [CustomAction(Theme = DisplayColorTheme.Warning, SectionName = "Danger", DisableWhenMemberIsSet = nameof(Armed), DisableWhenSetTo = true)]
    public ConfigurationActionResult SafeAction()
        => new("Safe.", DisplayColorTheme.Warning);

    /// <summary>Changes the document it was handed.</summary>
    [Display(Name = "Disarm")]
    [CustomAction(Theme = DisplayColorTheme.Important)]
    public ConfigurationActionResult DisarmAction(ConfigurationActionContext<CustomActionsConfiguration> context)
    {
        context.Configuration.Actions.Armed = false;
        return new(context.Configuration);
    }
}
