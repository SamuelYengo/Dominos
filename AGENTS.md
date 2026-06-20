# WebGL build and deploy

This project publishes the game to GitHub Pages by building WebGL locally and pushing the output in `docs/`. Do not use Unity cloud CI for this repo.

Live site: https://samuelyengo.github.io/Dominos/

## Prerequisites

- Unity Hub with editor `6000.0.30f1` installed
- Unity Personal license activated locally (sign in to Unity Hub)
- Close the Unity Editor before running a batch build if the Dominos project is open

Default Unity path used by the build script:

`C:\Program Files\Unity\Hub\Editor\6000.0.30f1\Editor\Unity.exe`

## Build locally

From the repo root in PowerShell:

```powershell
.\scripts\build-webgl.ps1
```

The script:

- Runs Unity in batch mode
- Calls `WebGLBuild.Build` in `Assets/Editor/WebGLBuild.cs`
- Writes output to `docs/`
- Creates `docs/404.html` and `docs/.nojekyll` for GitHub Pages

Build logs are written to `Logs/webgl-build.log`.

Optional custom output path:

```powershell
.\scripts\build-webgl.ps1 -OutputPath docs
```

Optional custom Unity path:

```powershell
.\scripts\build-webgl.ps1 -UnityPath "C:\Path\To\Unity.exe"
```

## Publish to GitHub Pages

After a successful local build:

```powershell
git add docs
git commit -m "publish webgl build"
git push origin main
```

Deployment is handled by `.github/workflows/deploy-pages.yml`, which uploads `docs/` to GitHub Pages when `docs/` changes on `main`.

## What not to do

- Do not add Unity cloud build workflows that require `UNITY_LICENSE` secrets. Unity 6 Personal uses entitlement licensing and does not provide a reliable `.ulf` file for GameCI.
- Do not commit `Builds/`; it is gitignored. Only commit `docs/`.
- Do not change GitHub Pages source away from GitHub Actions deploy unless the user asks.

## Project notes

- WebGL compression is disabled in `WebGLBuild.cs` for GitHub Pages compatibility.
- Tutorial videos for WebGL live in `Assets/StreamingAssets/` and are loaded at runtime by `Assets/Scripts/WebGLVideoSource.cs`.
- If the site shows README text instead of the game, `docs/` is missing or stale and needs a fresh local build pushed to `main`.

## Verify

1. Confirm `docs/index.html` exists after the build.
2. After push, check the `Deploy GitHub Pages` workflow in GitHub Actions.
3. Open https://samuelyengo.github.io/Dominos/ and confirm the Unity WebGL player loads.
