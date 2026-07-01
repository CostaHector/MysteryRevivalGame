# AGENTS.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**神秘复苏 (Mystery Revival)** — a 2D game built in **Godot 4.7** with **C# (.NET 8.0)**. Internal assembly/solution name is `GhostDomain`. Renderer is Forward+ (D3D12 on Windows); 3D physics uses Jolt.

- Window: 1024×768, `canvas_items` stretch, `expand` aspect.
- Target frameworks: `net8.0` desktop, `net9.0` for Android (`GodotTargetPlatform == android`).

## Build & Run

- Open in the Godot 4.7 **.NET/Mono** editor build; press F5 to run, or use the editor's Build button.
- CLI build: `dotnet build GhostDomain.csproj`. Build outputs land in `.godot/mono/temp/bin/` (gitignored).
- CLI run: `godot --path .` (requires a .NET-enabled Godot binary on PATH).
- No test suite, linter, or CI is configured.
- `addons/godot_mcp/` is a third-party MCP-server plugin — do not treat it as game code.

## Architecture: code-first, Main.cs as hub

The defining convention of this codebase: **the scene tree is built almost entirely in C# `_Ready()` methods, not in `.tscn` files.** The `.tscn` files are intentionally minimal — most nodes (sprites, cameras, UI, players) are `new`'d and `AddChild`'d at runtime. When adding features, follow this pattern rather than building rich scenes in the editor.

Two exceptions carry real scene data and *are* edited in the editor:
- `BornRoom.tscn` — holds the `NavigationPolygon` (walkable area), `FrontDoorArea` (Area2D + CollisionPolygon2D), and `Background`.
- `HeadsUpDisplay.tscn` — the welcome/title screen with the wired `StartButton` signal.

### Runtime flow

1. **`Main.tscn`** (`uid://c64n8jijvjwc`) is the entry scene → root node runs `Main.cs`.
2. `Main._Ready()` builds the cover background + `Camera2D`, then instantiates `HeadsUpDisplay.tscn` (title screen).
3. `HeadsUpDisplay` StartButton → `_on_start_button_button_down()` → hides itself, calls `Main.NewGame()`.
4. `Main.NewGame()` hides the cover and instantiates the gameplay nodes as children of `Main`: `BornRoom` (from tscn), `PlayerSprite`, `PlayerInteractDisplay`, `Backpack`.
5. **Scene transitions** swap sibling nodes under `Main` while keeping the player/HUD: `BornRoom.GotoDeprecatedHouse()` adds a `DeprecatedHouse` node and `QueueFree()`s itself.

Because everything hangs off `Main`, cross-node lookups use `GetParent<Main>().GetNode<PlayerSprite>("PlayerSprite")`. Node **names are load-bearing** — nodes set `Name` explicitly in `_Ready()` (e.g. `"PlayerSprite"`, `"CoverBackground"`) and other nodes fetch by that string. Renaming a node means updating its lookups.

### Key nodes

- **`PlayerSprite`** (`Sprite2D`) — WASD movement *and* click-to-move auto-navigation via a child `NavigationAgent2D`. Movement is gated by `CanMove` (set false while the backpack is open — game is *not* paused, so NPCs could still act). `_navMap` is validated with `IsNavReady()` before querying, because `NavigationServer2D` returns `(0,0)` until the map's first sync completes and a region is registered. `Offset` shifts the sprite so `Position` = the character's feet (for navmesh snapping). Emits `ArrivedAtTarget` when auto-nav finishes.
- **`BornRoom`** (`Node2D`) — wires `FrontDoorArea` hover/click; on click, moves the player to the door center (computed from the collision polygon AABB, *not* the Area2D position) and transitions on `ArrivedAtTarget`.
- **`PlayerInteractDisplay`** (`CanvasLayer`) — top status bar + bottom 1×9 hotbar, built entirely in code. Mouse wheel changes the selected slot.
- **`Backpack`** (`CanvasLayer`) — 9×4 grid, toggled by `open_backpack`/`exit`; always resident, shown/hidden via `Visible`.
- **`DeprecatedHouse`** (`Node2D`) — second location, background-only so far.

## Input Map (project.godot)

All actions use physical keycodes, 0.2 deadzone: `move_up`=W, `move_down`=S, `move_left`=A, `move_right`=D, `open_backpack`=E, `exit`=Escape.

## Code Conventions

- Node classes are `public partial class : <GodotType>` (required for Godot C# source-gen).
- `using Godot;` + `using System;` at top; PascalCase for public tunables (e.g. `Speed`).
- Prefer `Mathf.*` over `System.Math`; prefer Godot idioms (`GD.Load<T>`, `EmitSignal(SignalName.X)`).
- **Comments are in Chinese** — match this when editing existing code.
- Assets live in `asserts/` (note the spelling); load with `GD.Load<Texture2D>("res://asserts/...")`.

## Godot Housekeeping

- **Never edit or commit `.godot/`** — engine-generated cache (gitignored, along with `/android/`).
- Source-of-truth: `*.cs`, `*.tscn`, `project.godot`, `*.csproj`, `*.sln`, `*.svg`, `*.import`.
- Don't hand-edit `.uid` files or scene `uid://` references; let the editor manage them.
- When adding a script that needs a scene, either attach it in a `.tscn` or (per this project's style) instantiate it in code from a parent's `_Ready()`.
