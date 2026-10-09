# Completed usability worktree cleanup

9 October 2026. The separate merger preserved remaining local evidence and removed only `work/usability-worktrees/group` and `work/usability-worktrees/motion`. Integration began at `2146f2d817319c3d19757caad2202e359cce0ace` on `integration/usability-v1.2`.

The earlier [group](../group-usability-merge/report.md) and [motion](../fluid-interactions-merge/report.md) archives remain unchanged. Additional regression screenshots and diagnostic logs outside their selected manifests were retained in new ignored directories. Each archive preserves the complete worktree `artifacts/`, `game/Logs/`, `game/Temp/` and `game/UserSettings/` trees with original relative paths. The generated Library and test binary caches were disposable; the independent Newtonsoft runtime already remains outside these worktrees.

| Worktree | Retained branch and integrated head | New archive under primary checkout | Files | Bytes |
|---|---|---|---:|---:|
| group | `implement/group-usability` · `25b82d7e6906e7d432752813b1e66138e80af9d7` | `artifacts/usability-worktree-retirement-20261009T090033Z/group/` | 224 | 21,185,469 |
| motion | `implement/fluid-interactions` · `f07419bbbfd157c0072e61ab59f0c187d6e4ddfd` | `artifacts/usability-worktree-retirement-20261009T090033Z/motion/` | 173 | 22,452,382 |

The [group manifest](group-manifest.json) and [motion manifest](motion-manifest.json) list every retained path, byte count and SHA256. All source and copied files matched immediately after copying and again before removal. The complete archive inventories, sizes and hashes were rechecked afterward; no mismatch was found. Counts exclude each archive's own manifest. Derived total-byte fields were calculated from the verified entries before committing; source evidence files were not modified.

Immediately before removal, both worktrees still had their expected branch/head, no tracked or untracked changes, and only the known ignored evidence/cache directories. Both heads were ancestors of integration HEAD. Resolved absolute targets were exact children of `work/usability-worktrees/`; ancestor and recursive checks found no reparse points. Read-only process inspection found no Unity Editor using either exact `game` project path. The active final-corrections Editor, PID 215936, was left running and its worktree was not inspected or changed.

Both removals completed using `git -c core.longpaths=true worktree remove --force` after these guards. Force applied only to the explicitly inventoried ignored files and disposable generated caches; the procedure rejected unexpected tracked, untracked or ignored paths. Both branch references remain at their recorded heads. [Cleanup record](cleanup.json) preserves the checks, archive identities and removal timestamps.

No source changes, Editor control, ADB, build, test, tracker or push actions were performed. This housekeeping adds no new gameplay or release acceptance claim.
