this is my domino game!

## WebGL release build

Pushes to `main` run `.github/workflows/webgl-release.yml`.

The workflow:

- builds the Unity project as WebGL using Unity `6000.0.30f1`
- deploys the WebGL output to GitHub Pages
- creates a GitHub Release tagged `webgl-<run number>` with a zipped WebGL build

Before the workflow can build, activate a free Unity Personal license in Unity Hub.
Then add these repository secrets:

- `UNITY_LICENSE`: the contents of `C:\ProgramData\Unity\Unity_lic.ulf`
- `UNITY_EMAIL`: the email address for your Unity account
- `UNITY_PASSWORD`: the password for your Unity account
