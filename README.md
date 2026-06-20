this is my domino game!

## WebGL release build

Pushes to `main` run `.github/workflows/webgl-release.yml`.

The workflow:

- builds the Unity project as WebGL using Unity `6000.0.30f1`
- deploys the WebGL output to GitHub Pages
- creates a GitHub Release tagged `webgl-<run number>` with a zipped WebGL build

Before the workflow can build, add these repository secrets in GitHub under Settings → Secrets and variables → Actions:

- `UNITY_LICENSE`: the full contents of `C:\ProgramData\Unity\Unity_lic.ulf`
- `UNITY_EMAIL`: your Unity account email
- `UNITY_PASSWORD`: your Unity account password

If you do not have a license file yet, run the `Unity Activation Request` workflow once, download the `.alf` file from the run artifacts, activate it in Unity Hub, then copy the generated `Unity_lic.ulf` into the `UNITY_LICENSE` secret.

## Local WebGL build

```powershell
.\scripts\build-webgl.ps1
```

The local script writes to `Builds/`. Unity still needs a free Personal license activated locally before the build can run.
