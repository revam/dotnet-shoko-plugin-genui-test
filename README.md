# Shoko GenUI Test Plugin

A plugin holding nothing but forms, for trying out how a client renders Shoko's
generated UI (`UiDefinition`). It is never released: there is no manifest and no
release workflow. Its code doubles as a set of examples of every form feature.

What it adds: seven configurations, one per level of reactivity, each saying in
its description what a tester should see happen, and two executable actions.

1. **Save Only**: no hooks, conditions or server calls until Save. Tabs of
   every element (string, boolean, integer, nullable integer, float, enum,
   flags enum, password, text area, code editor) with ranges, lengths,
   patterns, denied values, an environment variable, a restart flag and
   advanced, read-only and hidden members; flat lists, an enum checkbox list,
   complex lists as tabs, a dropdown and inline, dictionaries keyed by strings
   and enums, of complex values and of lists; selects as a flat list, as
   checkboxes and grouped; a badge, the default section and a described
   floating section.
2. **Conditions**: client-side reactivity only. A member shown, hidden, made
   read-only or disabled by every condition operator, and a section drawn as a
   checkbox. Nothing is sent to the server.
3. **Custom Actions**: buttons in every position and theme, conditional ones,
   one in a floating section, one enabled only with changes, and actions that
   edit the draft, return field errors, save and ask for a refresh, ask for the
   save message, redirect, and answer with several messages.
4. **Server Options**: `[OptionsProvider]` lists computed from the unsaved
   draft, labelled, shared by two members, asynchronous, static, on a list, on
   list entries, on a dictionary's keys and values (each with its own
   provider) and on the entries of a dictionary's lists; a plugin type with a
   `TypeConverter`, labelled through it; a plugin type parsable from text; and
   a provider refusing the draft with a field error.
5. **Live Edit**: handlers per event (edited, focused and unfocused, clicked
   and view changed, new value for a row being added), narrowed to the members
   they watch, a nested handler running before the outer one, and a member no
   handler watches, which sends nothing.
6. **Lifecycle Hooks**: New, Load, Validate and Save hooks, with the default
   save button replaced by the section's own.
7. **Combined**: a live edit sets a value a condition reads, server options
   follow the edited value, and a button runs the same chain.

The actions are `GenUI Test: Global`, whose parameters cover most element kinds,
whose validation refuses on request and whose options are shared by two
parameters, and `GenUI Test: Series`, whose options are the titles of the series
it runs on. Running either only writes to the log.

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
