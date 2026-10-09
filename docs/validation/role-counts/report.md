# Free base-mode role counts — ticket #21

9 October 2026. Implementation branch `implement/role-counts`, based exactly on integration `96105ee`. [Specification](../../specs/role-counts.md); [ticket](../../specs/role-tickets/21-free-role-counts.md). No Kings round-starter behavior, release version, APK/AAB, device installation, tracker closure or push belongs to this handoff.

## Implemented contract

- Quick and Classic have separate durable manual Undercover/White pairs, Auto, pure White compositions and the shared strict initial-majority cap. Complete-pair commands are atomic. Shrink/grow and remove/undo retain desired counts with the accepted deterministic effective mix.
- The existing Group surface has two count rows, a single-slot replacement action, public bounds, Kings' fixed-White explanation and temporary-adjustment text. The count area scrolls on constrained screens so the saved group and Start remain reachable. Explicit Edit, reorder and remove/undo remain intact.
- Quick confirms and reveals one participant at a time. Nonterminal evil catches continue straight to another selection. The first Civilian loses immediately; every adversary must be caught for the good win. Mistake/tie winners include already caught adversary teammates. Each White receives one persisted spoken guess; a correct guess awards all Whites alone.
- Base-mode words stay concealed until Result; no enforced extra clue round, discussion or bundled selection. Classic survivor-round and terminal behavior is preserved. Result omits the unassigned Undercover word for pure-White deals.
- UI vote callbacks carry match identity and progress; confirmation, correction, tie and continuation reject stale identities/progress. New DTOs stay in the existing Session boundary and IL2CPP preservation covers the complete assembly.
- V6 retains V1–V5 support, old finalized Quick results and legacy Classic White preference. New fields falsely tagged as old schemas, invalid count pairs, unsafe generations and failed writes retain the existing validation/recovery contract. Opening never rewrites or redraws a deal.
- DE/EN setup, help, outcomes, Glossary and interface documentation match the new rules.

## Verification

The agreed seams are public Session with actual saved directories and the production rendered app. No private rules methods are tested.

Session: **87/87 passed**, [final output](evidence/13-session-final.txt). The production Session sources are those committed at `1e3e41fb3ef2bb429efc111f362ddc9dbd231822`; subsequent source commits only touch rendered UI/test code. This includes every valid base-mode count composition (838 size/mode/pair combinations, including minimum groups and all pure-White mixes), each dealt and reopened, alongside the prior Kings/group/content/recovery corpus. The 520 authored bilingual word pairs still match the shipped catalog.

Targeted rendered TDD: atomic single-slot replacement **1/1** in both languages; sequential Quick with two Whites **1/1** in both languages. Their actual failing and passing results are preserved below. Large-text count reachability/accessibility was exercised at 360×616 in DE/EN for Quick 3, Classic 4, Classic 20 and Kings 6. Every count/Auto control is scrolled into its viewport; the replacement and full bounds copy are reachable too. Screenshots include top, White-row, single-slot replacement and scrolled bounds positions where applicable.

Unity compilation at source `30ec5b1` reports completed with no compile errors in [compile status](evidence/22-compile-status.json). Complete rendered-suite result and final visual inspection are recorded in the final handoff addendum below.

## TDD and corrections retained

- `01-settings`: missing public complete-pair API failed compilation; after implementation pure White deals reopen at Quick3/Classic4.
- `02-preferences`: checksum-valid negative saved counts were accepted before validation; the same test passes after the validator change. Preference isolation, shrink/undo/growth, Auto and failed writes also pass.
- `03-schema`: writer initially remained V5; the V6 envelope and pre-V6 field rejection made the test pass.
- `04-quick`: missing continuation API failed compilation; sequential progress and final pending White now pass.
- `05-winners`: mixed/pure-White Quick mistake and tie initially produced the wrong winner roles; original-deal role winners corrected it.
- `10-ui`: the real rendered one-slot replacement control was absent; the same test passes with its new atomic button.
- `11-quick-ui`: a nonterminal Quick catch lacked its direct continuation control; after implementation the full bilingual multiple-White sequence passes without early word disclosure.
- `12-stale-vote`: missing progress-guarded overloads failed compilation; earlier-accusation and wrong-match commands now reject without mutation.
- First complete Session run was 83/84: the synthetic V1 fixture had been created by relabelling a new save while leaving new-only fields. It now strips those fields to represent an actual legacy shape. The final 87/87 includes this fix plus explicit finalized-V5-Quick and failure/stale tests. This was a fixture correction, not removal of the old-schema defense.
- Layout red caught a 4 px Start-button overflow at 150% text. Allowing the settings viewport to shrink to 84 px fixed that specific case, while every 48 px control remains reachable by scrolling. A later accessibility assertion initially compared an existing helper's label-plus-space value to an unpadded tooltip; comparison now trims that helper representation.
- Root's visual review caught invalid large-text capture evidence despite the passing targeted test: the fixture enlarged persistent text a second time after an action, producing 225% text on one capture and inconsistent sizing between scenarios. `0fac1ec` removes the second enlargement. Final capture assertions check actual mode text at 22.5 px and group-title text at 27 px, alongside viewport and control bounds, while retaining 150% scaling throughout. The earlier `17-layout-result.json` is retained as historical output but is **not valid visual acceptance evidence**. Root preserved the original bad screenshots; copies are retained in [invalid-captures](evidence/invalid-captures).
- The first completed full rendered suite [18](evidence/18-unity-full-result.json) passed 60/64 and caught four real layout regressions: the role area left too little room for a reorder destination and a large edited row, and crowded the empty/minimum-group guidance at 150%. `b705da6` bounds the role settings to 136 px, removes the redundant description when the group is ready, and shows the role editor only when the group can start. The existing GroupUsability suite then passed 6/6 and Polish passed 9/9. No existing assertion was weakened for those failures.
- A missing LINQ import in the optional pure-White result-word suppression caused a compile failure before the full suite could run; it is corrected separately in `2667fd0`. Two dispatched full runs were cancelled/aborted around that compile/reload transition and are not counted as passing tests. Their outputs and Editor logs remain retained.

## Environment and limits

Unity 6000.3.25f1 and the existing Pipeline 0.8.0 were used under the exclusive project lease. A first sandbox Editor/its licensing child held the real username's pipe without suitable entitlement visibility; Root stopped only those identified owned processes. The subsequent user-context Editor 246340 started and compiled normally. No tool or license installation/change was required. The sandbox and failed user-start logs remain under `artifacts/role-counts` for preservation.

The Editor log also contains existing obsolete `PreventDefault` warnings, missing ThemeStyleSheet warnings in the older Classic and Kings foundation test fixtures, and Unity Editor Search index exceptions during reload. These are distinct from runtime feature failures. No Console history was cleared. Runtime-generated Inter and Emoji font-asset changes are not part of the patch.

This work does not claim native update acceptance, physical-phone performance, new motion improvements or Store readiness. Root retains ADB and performs native acceptance after #22/#23. The count area requires scrolling on the smallest screen with 150% text; top-only screenshots do not prove all controls visible simultaneously.

## Final handoff

The final unchanged rendered source/test pin is **`30ec5b14fe1dafea7adb4ca81b349e047633d194`**. The complete PlayMode suite passed **64/64**, zero skipped or inconclusive, in **221.72 seconds**: [full result](evidence/22-unity-full-result.json). This includes the existing group reorder/edit/scroll, privacy, content, interrupted reveal and Kings behavior checks as well as the new base-mode behavior. Together with Session **87/87**, there are no failing tests remaining in this handoff. No source changes followed the successful full run.

The implementer actually inspected final DE Quick 3 top, White-row, replacement and limit captures; DE Kings 6 limits; EN Classic 20 top/limits; and EN Classic 4 replacement. Root independently inspected DE Quick 3 top/limits and both added controls, DE Kings 6 limits, EN Classic 20 limits and EN Classic 4 replacement. Both inspections found readable header/modes/Start, reachable White and replacement controls, and complete limit copy. This is visual inspection of rendered production UI, with native Android 150% acceptance still delegated to Root.

Final [screenshots](evidence/screenshots) and eight `role-counts-*.txt` metric files are checked in. Representative [DE Quick metrics](evidence/role-counts-German-Quick-3.txt): 360×640 render texture, root 360×616, mode font 22.5 px, group title 27 px, roles viewport 84 px high, players viewport 110 px high, Start at y526–584. The White/replacement/limit screenshots capture separate scroll positions; they do not imply simultaneous visibility.

The owned Editor was verified ready and stopped before shutdown ([status](evidence/23-editor-before-stop.json), [identity](evidence/23-owned-editor-identity.json)). Only process 246340 for this exact worktree was stopped; the exclusive Unity lease was returned to Root. Generated font differences were preserved under `artifacts/role-counts/23-generated-font-diff.txt` and restored, leaving no runtime asset changes. The worktree was merged against the current integration tip `96105eea8bb1a7f63d767e27993bc6054f57985f` (already up to date). No device, version, signing, release package, remote branch or issue status was changed.

Reproduction from this worktree uses the existing installed Editor/Pipeline and public seams:

```powershell
dotnet run --no-restore --project tests/Session.Tests/Session.Tests.csproj -p:NewtonsoftJsonAssembly=C:/Users/lucac/Documents/Codex/2026-10-06/sie/outputs/word-deduction/artifacts/repro-runtime/Newtonsoft.Json.dll
unity command run_tests --project-path ./game --caller plugin --skill unity-feature-implementation --mode playmode --async_tests true --format json
unity command test_status --project-path ./game --caller plugin --skill unity-feature-implementation --format json
```

Start Editor only after acquiring the exclusive lease, explicitly using Unity 6000.3.25f1 and normal user context. Full retained logs and intermediate output remain in this worktree's ignored `artifacts/role-counts`; Root must preserve those before archiving the worktree.
