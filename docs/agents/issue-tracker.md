# Issue tracker: GitHub

Issues and specifications for this repo live in GitHub Issues for `giarrel/word-deduction`.
Infer the repository from `git remote -v` when operating from a checkout.

Use the connected GitHub tools when available. Otherwise, use an authenticated `gh` CLI.

## Conventions

- Create an issue with a title and body in this repository.
- Read the issue body, labels, state, assignees, and comments before acting on a ticket.
- List open issues with the appropriate label filters; read comments for relevant tickets.
- Apply or remove the labels mapped in `docs/agents/triage-labels.md`.
- Record the outcome in the issue before closing it.
- Use structured tool arguments for multiline bodies. With `gh`, save the exact body to a temporary file and pass `--body-file`.

When using the CLI, the corresponding operations are `gh issue create`, `gh issue view <number> --comments`, `gh issue list`, `gh issue comment`, `gh issue edit`, and `gh issue close`.

## Pull requests as a triage surface

**PRs as a request surface: no.**

GitHub issues and pull requests share a number space. Resolve an ambiguous `#<number>` by checking whether it identifies an issue or a pull request before choosing the operation.

## When a skill says "publish to the issue tracker"

Create a GitHub issue in `giarrel/word-deduction`.

## When a skill says "fetch the relevant ticket"

Read the corresponding GitHub issue, including comments and labels.

## Wayfinding operations

Used by `/wayfinder`. The map is a single issue with child issues as tickets.

- Map: an issue labelled `wayfinder:map`, holding the Notes / Decisions-so-far / Fog body.
- Child ticket: link it to the map as a GitHub sub-issue when supported. Otherwise, add it to a task list in the map body and put `Part of #<map>` at the top of the child body. Use `wayfinder:research`, `wayfinder:prototype`, `wayfinder:grilling`, or `wayfinder:task` as appropriate.
- Blocking: use native GitHub issue dependencies when supported by the available tools. Otherwise, put `Blocked by: #<number>, #<number>` at the top of the child body. A ticket is unblocked when every blocker is closed.
- Frontier: inspect the map's open children, exclude assigned tickets and tickets with open blockers, and choose the first remaining ticket in map order.
- Claim: assign the ticket to the driving developer before starting its work.
- Resolve: comment with the outcome, close the child issue, and append a brief finding with a link to the map's Decisions-so-far.
