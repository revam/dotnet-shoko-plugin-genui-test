using System;
using Shoko.Abstractions.Plugin;

namespace Shoko.Plugin.GenUiTest;

/// <summary>
///   A plugin holding nothing but forms, for trying out the generated UI.
/// </summary>
public class Plugin : IPlugin
{
    /// <inheritdoc />
    public Guid ID { get; private init; } = new("eb286d61-825a-4d8f-a09c-6594ff6bf6c5");

    /// <inheritdoc />
    public string Name { get; private init; } = "GenUI Test";

    /// <summary>
    ///   What the plugin is for, read at build time.
    /// </summary>
    public string Description { get; private init; } = """
        Every generated UI feature in one place, for testing clients. Its actions only log and answer.
        """;
}
