this is my domino game!

Play it here: https://samuelyengo.github.io/Dominos/

## Publish WebGL to GitHub Pages

Build locally, copy to `docs/`, commit, and push:

```powershell
.\scripts\build-webgl.ps1 -OutputPath docs
git add docs
git commit -m "publish webgl build"
git push origin main
```

Pushing `docs/` triggers `.github/workflows/deploy-pages.yml` and updates the live site.

## Optional: build on GitHub Actions

`.github/workflows/webgl-release.yml` can build in the cloud, but it needs `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD` repository secrets. Run it manually from the Actions tab once those are set.
