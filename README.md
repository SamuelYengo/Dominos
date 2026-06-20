this is my domino game!

Play it here: https://samuelyengo.github.io/Dominos/

## Publish WebGL to GitHub Pages

Build locally, commit `docs/`, and push:

```powershell
.\scripts\build-webgl.ps1
git add docs
git commit -m "publish webgl build"
git push origin main
```

Pushing `docs/` triggers GitHub Actions and updates the live site.

See `AGENTS.md` for the full WebGL publish workflow.
