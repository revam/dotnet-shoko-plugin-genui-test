# Shoko GenUI Test Plugin

A plugin holding nothing but forms, for trying out how a client renders Shoko's
generated UI (`UiDefinition`). It is never released: there is no manifest and no
release workflow. Its code doubles as a set of examples of every form feature.

What it adds:

- **The `GenUI Test` configuration**, laid out as tabs:
  - *General*: loose members gathered into the default section, a described
    floating section (`Login`), a badge and a value the validation hook refuses
    (`invalid`).
  - *Primitives*: every scalar element (string, boolean, integer, nullable
    integer, float, enum, flags enum, password, text area, code editor) with
    ranges, lengths, patterns, denied values, an environment variable, a
    restart flag, and advanced, read-only and hidden members.
  - *Collections*: flat lists, an enum checkbox list, complex lists as tabs, a
    dropdown and inline, and dictionaries keyed by strings and enums, of
    complex values and of lists.
  - *Conditions*: a member shown, hidden, made read-only or disabled by every
    condition operator, and a section drawn as a checkbox.
  - *Options*: server-listed options (`[OptionsProvider]`): computed from the
    unsaved draft, labelled, shared by several members, asynchronous, static,
    on a list and on list entries.
  - *Selects*: `SelectComponent<T>` as a flat list, as checkboxes and grouped.
  - *Actions*: custom actions in every position and theme, conditional ones,
    one that edits the document, and a live-edit handler.
- **Two executable actions**: `GenUI Test: Global`, whose parameters cover most
  of the above and whose validation refuses on request, and
  `GenUI Test: Series`, whose options are the titles of the series it is run
  on. Running either only writes to the log.

## Building

The plugin needs the abstractions from the Shoko branch carrying the generated
UI, which is not on a published package yet. Build it against a local Shoko
checkout on that branch:

```sh
dotnet build -c Release -p:ShokoRepositoryPath=/path/to/ShokoServer
```

That references the checkout's `Shoko.Abstractions` and its analyzers as
projects. Without `ShokoRepositoryPath` it falls back to the
`Shoko.Abstractions` package named by `ShokoAbstractionsVersion` in
`Directory.Build.props`, which only works once a package carrying these
features is published.

## Trying it

Copy the build output into a development server's plugins folder, in a folder
of its own, and restart the server:

```sh
mkdir -p <server data>/plugins/Shoko.Plugin.GenUiTest
cp source/bin/Release/net10.0/Shoko.Plugin.GenUiTest.* <server data>/plugins/Shoko.Plugin.GenUiTest/
```

The configuration then shows up among the plugin's settings, and the actions
among the global and series actions. Do not install it on a server you use:
nothing in it is harmful, but nothing in it is useful either.
