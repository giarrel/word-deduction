# Spec verification — code10 visual correction

Read-only review of `e106ce6f388a4e2dce834e4c2fe2b7092b61d47a..bd7b618c957051d3ee61321a6ed3d275a5302a93` in the final-corrections worktree against Specs #17/#18.

**0 new Spec findings.** The font-dependent grip becomes a non-focusable VisualElement containing three rounded USS bars. Its 44×48 minimum touch area, localized tooltip, existing reorder manipulator and public accessibility exclusion remain; the bars use `PickingMode.Ignore`, so they do not steal the handle's pointer target. Explicit Edit and the move-up/down alternative remain available. This satisfies #17's “Give reorder a distinct handle” and “Provide accessible move-up/move-down actions in editing” decisions without changing group or gesture rules.

The shorter DE/EN hint removes redundant Edit explanation while retaining the reorder instruction and translated Edit buttons, consistent with #17's simple, localized group setup. I viewed the retained German Kings render: the grip is light, the hint occupies one line, and names/Edit controls remain readable. This is an offscreen Unity image, not native-device acceptance.

Recorded post-change results are GroupUsability 6/6 (2.7 s) and Motion 4/4 (0.89 s), without failures or skips. I inspected those results; I did not run tests. The 60px row minimum is retained, not newly reduced. Session, persistence, role rules and motion implementation are unchanged by this correction.

Version 1.2.0/code10 is consistent across AppBuild, ProjectSettings, release-inspector defaults and reproduction documentation, allowing a monotonic update over the already installed code9 candidate.

Native code10 installation/persistence, system-font/keyboard/accessibility checks, paired frame measurements, continuous motion inspection and reduced-motion/privacy verification remain the coordinator's acceptance gates. This review makes no physical-device performance claim and does not mark pending role-count issue #11 complete.

No Editor, ADB, tests or repository mutations were performed by this reviewer; only this external report was written.
