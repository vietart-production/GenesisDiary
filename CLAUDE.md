# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

GenesisDiary is a Unity project (Editor version **6000.0.64f1**, see `ProjectSettings/ProjectVersion.txt`) using the Universal Render Pipeline (URP). It was scaffolded from Unity's default 3D (URP) template and is currently in an early/empty state:

- `Assets/Scripts/GenesisManager.cs` is the only gameplay script, and it is an empty `MonoBehaviour` stub.
- `Assets/Scenes/SampleScene.unity` is the only scene (the default template scene).
- `Assets/TutorialInfo/` holds Unity's built-in "Readme" asset/editor used by the default template's welcome screen — not project documentation.
- There are no custom Assembly Definitions (`.asmdef`); scripts compile into the default `Assembly-CSharp` / `Assembly-CSharp-Editor` assemblies (see the two `.csproj` files at the repo root).
- There is no `Tests` folder yet, even though `com.unity.test-framework` is a listed dependency.
- Git is initialized with a Unity-specific `.gitignore` (excludes `Library/`, `Temp/`, `Logs/`, `UserSettings/`, generated `.csproj`/`.sln`/`.slnx`, etc.) and Git LFS tracking (`.gitattributes`) for binary art assets — images, 3D models, audio, video, fonts.

Because the project is essentially a blank slate, there is no established architecture to preserve yet — new scripts, folder structure, and scene organization can be set up as needed.

## Working with this project

This is a Unity Editor project, not a CLI/npm-style toolchain — there are no `package.json` scripts. Standard ways to interact with it:

- **Open/edit in the Unity Editor** (version 6000.0.64f1, install via Unity Hub if not present) for scene work, prefab work, and Inspector-driven configuration.
- **Edit C# scripts** directly (`Assets/Scripts/...`); Unity recompiles them automatically when the Editor regains focus.
- **Command-line batch mode** (no Editor UI) can be used for automation, e.g.:
  - Build: `Unity.exe -batchmode -projectPath . -buildTarget <target> -quit -logFile Logs/build.log`
  - Run tests once a `Tests` assembly exists: `Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Logs/results.xml -quit`
- `GenesisDiary.slnx` / `GenesisDiary.sln` (Unity-generated) open the C# scripts in Visual Studio / Rider; regenerate them from the Editor (Preferences → External Tools → Regenerate project files) rather than hand-editing, since Unity overwrites them.
- **Unity MCP server is connected** (`.mcp.json` registers the relay binary at `~/.unity/relay/`, per Unity's `com.unity.ai.assistant` package). This lets Claude Code drive the running Unity Editor directly — manage scenes/GameObjects/assets/shaders, create and edit scripts, capture Scene/Game view screenshots, read the console, profile performance, and run arbitrary Editor C# via `Unity_RunCommand`. All tools are enabled in **Project Settings → AI → Unity MCP Server → Tools**; the Unity Editor must be open and the bridge running for these tools to work.

## Key package dependencies (`Packages/manifest.json`)

Worth knowing when adding features, since they indicate available APIs:
- `com.unity.render-pipelines.universal` — URP rendering (assets under `Assets/Settings/`, e.g. `PC_RPAsset`/`Mobile_RPAsset` render pipeline assets and renderers).
- `com.unity.inputsystem` — new Input System (not the legacy `Input` class) is available.
- `com.unity.ai.navigation` — NavMesh/AI navigation.
- `com.unity.timeline` — Timeline/cutscene sequencing.
- `com.unity.visualscripting` — Visual Scripting graphs.
- `com.unity.ai.assistant` / `com.unity.ai.inference` — Unity AI Assistant and Unity Inference Engine (formerly Sentis) for in-editor AI tooling / ML model inference.
- `com.unity.test-framework` — Unity Test Framework (NUnit-based EditMode/PlayMode tests), though no tests exist yet.
