this is my domino game!

## WebGL release build

Pushes to `main` run `.github/workflows/webgl-release.yml`.

The workflow:

- builds the Unity project as WebGL using Unity `6000.0.30f1`
- deploys the WebGL output to GitHub Pages
- creates a GitHub Release tagged `webgl-<run number>` with a zipped WebGL build

Before the workflow can build, add a repository secret named `UNITY_LICENSE`.
