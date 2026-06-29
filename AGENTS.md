# AGENTS.md

Project guidance for AI agents working in this repository.

## Project Overview

**GhostDomain** is a Godot 4.7 game project written in C# (.NET 8.0). It is in an early stage with a single playable scene containing a movable Sprite2D.

- Engine: Godot 4.7 (Forward+ renderer, D3D12 on Windows)
- Language: C# via Godot.NET.Sdk 4.7.0
- Target frameworks: `net8.0` (desktop), `net9.0` (Android)
- Physics: Jolt Physics (3D)
- Window stretch: `canvas_items` mode, `expand` aspect

## Repository Layout

```
project.godot          # Engine config: app name, main scene, input map, rendering
GhostDomain.sln        # Visual Studio solution
GhostDomain.csproj     # Godot .NET project (Godot.NET.Sdk 4.7.0)
MySprite2D.tscn        # Main scene (uid://b5a6eepb053qw), root node MySprite2D
MySprite2D.cs          # Script for the root Sprite2D; handles WASD movement
icon.svg               # Default project icon (used as the sprite texture)
.godot/                # Engine-generated cache (gitignored, do not edit)
```

## Main Scene & Entry Point

- The main scene is `MySprite2D.tscn` (referenced by `run/main_scene` in `project.godot`).
- The root node `MySprite2D` (type `Sprite2D`) loads `MySprite2D.cs` and uses `icon.svg` as its texture.

## Input Map

Defined in `project.godot` under `[input]`. All actions use a 0.2 deadzone and physical keycodes:

| Action      | Key | physical_keycode |
|-------------|-----|------------------|
| `move_up`    | W   | 87               |
| `move_down`  | S   | 83               |
| `move_left`  | A   | 65               |
| `move_right` | D   | 68               |

## Code Conventions

- C# files use `using Godot;` and `using System;` at the top.
- Node-derived classes are `public partial class` (required for Godot C# scripting).
- Public tunable properties use PascalCase (e.g., `Speed`, `AngularSpeed`).
- `Mathf.Pi` and other Godot math helpers are preferred over `System.Math`.
- Comments are written in Chinese (match this when editing existing code).
- Use `Position with { X = ... }` style record-init for `Vector2` updates.

## Build & Run

- Open the project in the Godot 4.7 editor (the .NET sdk is configured in `GhostDomain.csproj`).
- Build the C# solution from the editor (`Build` button) or via `dotnet build GhostDomain.csproj`.
- Run from the editor (F5) or `godot --path .` with the .NET-enabled editor build.
- Build outputs land in `.godot/mono/temp/bin/Debug/` (gitignored).

## Gitignore Notes

`.godot/` and `android/` are ignored. Source-of-truth files are `*.godot`, `*.cs`, `*.tscn`, `*.csproj`, `*.sln`, `*.svg`, and `*.import` metadata.

## Working With This Project

- Prefer editing existing files over creating new ones.
- Do not commit anything under `.godot/` — it is engine-generated.
- When adding scripts, register them via the editor or attach to a node in a `.tscn`; do not hand-edit UIDs.
- Keep the C# code consistent with the existing style in `MySprite2D.cs`.
