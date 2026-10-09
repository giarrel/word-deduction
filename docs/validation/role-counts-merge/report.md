# Free role counts: separate merge and evidence preservation

9 October 2026. Ticket #21 handoff `d34e48d171e4d24e85258ff3ea7dab467b079a22` merged without conflicts into `integration/role-counts-v1.3`, from `96105eea8bb1a7f63d767e27993bc6054f57985f`. Merge commit: **`a425f16e9981e328741968c74224bf458739f9c7`**. Both checkouts were clean at the authorized handoff; the implementation already contained the integration base. No production edits or conflict resolutions were made during this merge.

## Exact validated source

The `game`, `tools`, `tests`, `content` and `third-party` Git trees at the merge are identical to both the handoff and validated runtime/test source **`30ec5b14fe1dafea7adb4ca81b349e047633d194`**. [Tree verification](verification.json) records every tree ID. The remaining handoff commits contain documentation and evidence only.

The [implementer report](../role-counts/report.md) and retained outputs document **87/87 Session** and **64/64 rendered PlayMode**, zero skipped/inconclusive, the latter in221.72seconds. This merger did not rerun those tests or launch an Editor/device. Root and the implementer inspected the final rendered layouts as described in that report; this merger does not claim an additional visual inspection. Original invalid captures, layout failures and aborted runs remain preserved.

## Byte-preserved evidence

The [durable archive](../../../artifacts/role-counts-preservation/d34e48d171e4-20261009T181847Z/) contains **171 original files /19,839,942bytes**, separately from75 canonical committed Git files. Its [preservation inventory](preservation.json) maps source paths, archive paths, sizes and SHA256. Source and copied raw bytes were compared and rechecked; canonical archived Git blobs were also rehashed independently.

Preserved scope: all `artifacts/role-counts` raw test/Editor outputs; relevant `artifacts/kings-rules-cards/role-counts-*` metrics and screenshots; the implementer's working copies of `docs/validation/role-counts`; Root's `root-visual-check` report and original failed screenshots; and `environment-notes.md`. No worktree cleanup was performed.

The implementer's `evidence-sha256.txt` describes **pre-Git capture bytes**, not necessarily checkout-normalized text. All73 catalog entries match the original implementer working files; only49 match the committed Git blob SHA256 exactly. All73 working-file/Git-blob pairs are equal after CRLF-to-LF normalization. The original raw/working bytes and canonical Git blobs are therefore inventoried and retained separately. The24 textual hash differences are not represented as a universal exact-byte post-Git pass or silently rewritten.

## Remaining scope

Ticket #21 is integrated. Ticket #22 adds Kings starters; ticket #23 performs independent reviews, final native/update acceptance and APK/AAB build verification. This merge does not claim those later criteria, Store readiness or new physical-device performance. No Unity, ADB, package build, issue update, push or cleanup was performed by the merger.
