# Group usability merge — ticket #19

Merged `implement/group-usability` into `integration/usability-v1.2` with a separate merger and an explicit merge commit. Both checkouts were clean before integration, and the source contains the integration starting point.

- Integration parent: `af0cba1f605877dfd3ee672f869704a74a9d9174`.
- Implementer handoff: `25b82d7e6906e7d432752813b1e66138e80af9d7`.
- Final implementation source: `99b0bf04c236fc8f6c1467cf699d760de29c6acc`; the handoff adds only validation evidence and its Git attributes.
- Merge: `3d9687c6a9fd043148d22d6c7d1f19c692a2d45e`. No conflicts or production edits were necessary. Its complete tree equals the handoff tree, `3b411639f99510584cd3ab6c4dbe407c096ddc0a`.

[Verification](verification.json) records successful post-merge SHA256 and byte-count checks for all 70 selected files (1,768,010 bytes). Before merging, all 72 manifest-listed raw files (16,487,391 bytes), including generated font caches, were copied to ignored `artifacts/usability-group-implementation/`, preserving each manifest-relative path. Every source and copied file was checked; all copied hashes were verified again after merging. The source worktree remains available.

The [implementation report](../group-usability/report.md) remains the test evidence authority. Its last full Session run was **75/76**, with an intermittent Windows `File.Replace` failure in the unchanged corpus stress scenario. A focused corpus retry passed 1/1; the coordinator's separate unchanged code8 baseline corpus also passed 1/1, but neither result establishes the intermittent failure's cause. The last full rendered run was **50/53**; subsequent affected checks passed **9/9, 1/1 and 1/1**. No fresh full 76/76 or 53/53 result is claimed.

No tests were run during this merge because the coordinator was capturing native motion measurements and requested an uncontended host. No Unity, ADB, build, tracker, push or worktree-removal operations were performed. Final integration still requires independent review, full regression checks, motion integration and Android update, keyboard, accessibility, persistence and package acceptance.
