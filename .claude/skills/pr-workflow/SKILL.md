---
name: pr-workflow
description: Turn ProjectForge changes into a clean branch, commit, push, and PR workflow using the existing PR description skill.
---

# PR Workflow

Use this skill when asked to prepare ProjectForge changes for a pull request.

## Purpose

Take completed local changes through a clean PR workflow:
- validate or create branch
- review readiness
- prepare commit
- push branch
- generate PR title and body
- create PR if tooling allows

## Project Context

ProjectForge is a small .NET 10 Web API project. PRs should stay small, focused, and easy to review.

## Required Behavior

1. Check the current branch and working tree.
2. If needed, use the branch-management approach to create a clean feature/fix/chore branch.
3. Run a lightweight pre-PR review before pushing.
4. Prepare a clean commit message.
5. Stage and commit only the intended changes.
6. Push the branch to origin.
7. Generate the PR description using the existing `pr-description` skill.
8. Reuse the existing PR description format instead of inventing a new one.
9. If GitHub PR creation tooling is available, create the PR.
10. If PR creation tooling is not available, output:
- suggested PR title
- ready-to-paste PR description
- target branch if relevant

## PR Title Guidance

Keep the PR title:
- short
- clear
- aligned with the main change

Examples:
- `Add task update endpoint`
- `Add task status patch endpoint`
- `Improve login request validation`

## Guardrails

Do not:
- push unrelated changes
- create a PR for mixed-scope work without warning
- invent a PR description format if `pr-description` already exists
- skip review of obvious problems before pushing

## Output Expectations

Provide a concise summary of:
- branch used or created
- commit message used
- whether the branch was pushed
- PR title
- PR description
- whether the PR was created automatically or must be created manually

## Final Rule

Optimize for a clean, reviewable PR workflow that fits naturally into ProjectForge and reuses the existing `pr-description` skill.