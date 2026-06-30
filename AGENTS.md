# AGENTS.md

Project guidance for AI agents working in this repository.

## Project Overview

**GhostDomain**（神秘复苏）is a Godot 4.7 2D point-and-click room exploration game written in C# (.NET 8.0). Player walks between rooms, clicks doors to travel, and movement is constrained to walkable floor areas via a navigation mesh.

- Engine: Godot 4.7 (Forward+ renderer, D3D12 on Windows)
- Language: C# via Godot.NET.Sdk 4.7.0
- Target frameworks: `net8.0` (desktop), `net9.0` (Android)
- Physics: Jolt Physics (3D)
- Window: 1024×768, stretch mode `canvas_items`, aspect `expand`

## Repository Layout

```
project.godot          # Engine config: app name, main scene, input map, rendering
GhostDomain.sln        # Visual Studio solution
GhostDomain.csproj     # Godot .NET project (Godot.NET.Sdk 4.7.0)
Main.tscn              # Main scene (uid://c64n8jijvjwc) — root node Main (Node)
Main.cs                # Entry point: dynamic node creation, scene management, camera
PlayerSprite.cs        # Player Sprite2D: WASD + click-to-walk + nav mesh constraint
BornRoom.tscn          # BornRoom scene: Background, FrontDoorArea, NavigationRegion2D
BornRoom.cs            # BornRoom script: door click → walk → scene switch
DeprecatedHouse.tscn   # DeprecatedHouse scene (minimal, builds itself in _Ready)
DeprecatedHouse.cs     # DeprecatedHouse script: dynamically creates BackGround sprite
HeadsUpDisplay.tscn    # HUD CanvasLayer: title, creators, StartButton
HeadsUpDisplay.cs      # HUD script: StartButton → hide layout → Main.NewGame()
icon.svg               # Default project icon
asserts/               # Game art (textures referenced by res:// paths)
.godot/                # Engine-generated cache (gitignored, do not edit)
```

## Main Scene & Entry Point

- The main scene is `Main.tscn` (referenced by `run/main_scene` in `project.godot`).
- The root node `Main` (type `Node`) loads `Main.cs`.
- **`Main._Ready()` dynamically creates** (no tscn children): `CoverBackground` (Sprite2D with `MysteryRevivalCover.png`), `MainCamera` (Camera2D centered on cover), and instantiates `HeadsUpDisplay.tscn`.
- **`Main.NewGame()`** (called from HUD StartButton): hides cover, instantiates `BornRoom.tscn`, creates `PlayerSprite`.

## Game Flow

1. Launch → `Main._Ready()`: cover background + camera + HUD welcome page
2. Click "开始游戏" → `HeadsUpDisplay._on_start_button_button_down()` → hide `WelcomePageVLayout` → `Main.NewGame()`
3. `NewGame()`: hide cover, load `BornRoom.tscn`, create `PlayerSprite` as child of Main
4. In BornRoom: WASD moves player (constrained to nav mesh), click door → `PlayerSprite.MoveTo(doorCenter)` → on arrival `GotoDeprecatedHouse()`
5. `GotoDeprecatedHouse()`: creates `DeprecatedHouse`, adds to Main, `QueueFree()` on BornRoom (player and HUD survive)

## Scene Files

### Main.tscn
Minimal — just `Main` (Node) with `Main.cs` attached. All children created in code.

### BornRoom.tscn
Static children (designed in editor):
- `Background` (Sprite2D, `asserts/BornRoom.jpg`, `centered=false`, `position=(48,-40)`, `z_index=-1`)
- `FrontDoorArea` (Area2D, `position=(864,48)`)
  - `FrontDoorCollision` (CollisionPolygon2D, `position=(-56,256)`, polygon defines door shape)
- `NavigationRegion2D` (with baked `NavigationPolygon` — drawn to match floor area)

### DeprecatedHouse.tscn
Minimal — just `DeprecatedHouse` (Node2D) with `DeprecatedHouse.cs` attached. `BackGround` sprite created in `_Ready()`.

### HeadsUpDisplay.tscn
`HeadsUpDisplay` (CanvasLayer) with `WelcomePageVLayout` (VBoxContainer) containing:
- `GameTitle` (Label, "神秘复苏", font size 128)
- `CreatorsLabel` (Label, "创作者名单: Costa, Yang", font size 48)
- `StartButton` (Button, "开始游戏", font size 64)

Signal connection: `WelcomePageVLayout/StartButton.button_down` → `HeadsUpDisplay._on_start_button_button_down`.

## PlayerSprite.cs

Player is a `Sprite2D` (not `CharacterBody2D`) with:
- `Speed = 400` (pixels/sec)
- `StartPosition` (Marker2D, default `(500, 200)`)
- `[Signal] ArrivedAtTarget` — emitted when navigation finishes
- `Offset = (0, -Texture.GetHeight()/2)` — **feet are the Position anchor**, so nav mesh snap puts feet on floor
- `NavigationAgent2D` child (`PathDesiredDistance=4`, `TargetDesiredDistance=8`) for click-to-walk
- `_navMap` (RID) cached from `GetWorld2D().NavigationMap` for manual `MapGetClosestPoint` queries

### Movement modes (`_Process`)
1. **Start pending**: nav map not ready → wait for `IsNavReady()` (iteration id > 0 AND regions > 0 AND `MapGetClosestPoint` returns non-zero), then snap `StartPosition` to nearest walkable point
2. **Auto-navigation** (`_isMoving=true`): follow `_navAgent.GetNextPathPosition()`, emit `ArrivedAtTarget` on finish
3. **WASD**: `Input.GetVector("move_left","move_right","move_up","move_down")` → propose new position → `IsWalkable()` check via `MapGetClosestPoint` (tolerance 4px) → reject if off-mesh

### Key methods
- `MoveTo(Vector2 target)` — set nav target, start auto-walk
- `Start(Vector2 position)` — set initial position (with nav-map snap pending logic)
- `IsWalkable(Vector2 pos)` — true if pos is within `WalkTolerance` of nearest nav mesh point

## BornRoom.cs

- `_Ready()`: connect `FrontDoorArea.InputEvent` to handler
- `_on_exit_to_deprecated_house_input_event()`: on left-click, call `PlayerSprite.MoveTo(GetDoorCenter())` and subscribe to `ArrivedAtTarget`
- `GetDoorCenter()`: computes AABB center of `FrontDoorCollision.Polygon` (local) → `collision.ToGlobal(localCenter)`. **Do not use `FrontDoorArea.GlobalPosition`** — the Area2D node position is decoupled from its collision shape position.
- `OnPlayerArrivedAtDoor()` → `GotoDeprecatedHouse()`
- `GotoDeprecatedHouse()`: new `DeprecatedHouse()` added to Main, then `QueueFree()` self

## Camera & Background Fitting

`Main._Ready()` sets `MainCamera.Position = coverSprite.Texture.GetSize() / 2` and `MakeCurrent()`. There's no `FitCameraToBackground` helper in `Main` — BornRoom's `Background` uses `centered=false` at `(48,-40)` so its top-left aligns near origin, and the camera stays where the cover left it. If you add new rooms with different background sizes, you may need to recenter the camera on the new background.

## Input Map

Defined in `project.godot` under `[input]`. All actions use 0.2 deadzone and physical keycodes:

| Action      | Key | physical_keycode |
|-------------|-----|------------------|
| `move_up`    | W   | 87               |
| `move_down`  | S   | 83               |
| `move_left`  | A   | 65               |
| `move_right` | D   | 68               |

## Code Conventions

- C# files use `using Godot;` and `using System;` at the top.
- Node-derived classes are `public partial class` (required for Godot C# scripting).
- Public tunable properties use PascalCase (e.g., `Speed`).
- `Mathf` helpers preferred over `System.Math`.
- **Comments are written in Chinese** — match this when editing existing code.
- Use `Position with { X = ... }` style record-init for `Vector2` updates (note: `with` only supports `=`, not `+=`/`-=`).
- Godot C# lifecycle methods (`_Ready`, `_Process`, etc.) require the `override` keyword or the engine won't call them.
- Prefer **dynamic node creation via code** over static scene setup in the Godot editor (per user preference). Exception: BornRoom.tscn keeps Background/FrontDoorArea/NavigationRegion2D static so the editor can be used to visually position collision shapes and draw the nav polygon.

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
- Keep the C# code consistent with the existing style in `PlayerSprite.cs`.

## Known Pitfalls (lessons learned)

- `new ClassName()` creates an empty object **without** tscn-defined children; use `PackedScene.Instantiate<T>()` to load scene-defined children.
- `Area2D.InputEvent` won't fire if the Area2D's collision shape is positioned off-screen (negative Y when no Camera2D offset) — keep collision shapes within visible coordinates.
- `NavigationServer2D.MapGetClosestPoint` returns `(0,0)` if the map is empty OR if regions are registered but the navigation mesh hasn't been built yet (happens one iteration after region registration). Check `MapGetIterationId > 0` AND `MapGetRegions().Count > 0` AND non-zero return before trusting the result.
- `GetViewportRect()` is a `CanvasItem` method — for `Node`-derived classes use `GetViewport().GetVisibleRect().Size` instead.
- `Camera2D.Current = true` doesn't compile in C# — use `camera.MakeCurrent()`.
- `RID.IsValid` is a property (no parentheses) in Godot C#, not a method.
