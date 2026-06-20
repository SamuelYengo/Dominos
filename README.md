this is my domino game!

## Manual WebGL publish

Build the game locally into `docs/`, commit it, and push to `main`.

```powershell
.\scripts\build-webgl.ps1
git add docs
git commit -m "publish webgl build"
git push origin main
```

Then set GitHub Pages to serve from `main` / `docs`.

Unity still needs a free Personal license activated locally before the build can run.
