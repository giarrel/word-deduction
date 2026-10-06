# Domain docs

This repository uses a single-context layout: one root `GLOSSARY.md` and `docs/adr/` for architectural decisions.

## Before exploring

- Read the root `GLOSSARY.md` when it exists.
- Read ADRs under `docs/adr/` that affect the area being explored.
- If a root `GLOSSARY-MAP.md` is introduced later, follow its pointers to the glossaries relevant to the topic and check any corresponding context-scoped ADR directories.

When these files do not exist, proceed silently. The `/domain-modeling` skill creates documentation lazily as domain terms and architectural decisions are resolved.

## Use the glossary's vocabulary

Use defined domain terms in issue titles, proposals, hypotheses, tests, and code. If a needed concept is missing, check whether the project already uses a different term; record a real vocabulary gap for `/domain-modeling`.

## Surface ADR conflicts

If a proposal contradicts an existing ADR, identify the ADR and explain why the decision should be reconsidered.
