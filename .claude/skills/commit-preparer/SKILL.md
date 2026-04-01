---
name: commit-preparer
description: Prepare clean commits for ProjectForge by reviewing changes and generating focused commit messages.
---

# Commit Preparer

Use this skill when asked to prepare commits for ProjectForge changes.

## Purpose

Help turn completed local changes into a clean, reviewable commit.

## Project Context

ProjectForge is a small .NET 10 Web API project. Commits should be small, focused, and easy to understand in PRs.

## Rules

1. Inspect changed files before suggesting a commit.
2. Prefer one logical commit for one logical change.
3. Do not group unrelated work into a single commit unless the user explicitly wants that.
4. Keep commit messages short and clear.

## Commit Message Style

Prefer imperative, PR-friendly commit messages such as:
- `Add task update endpoint`
- `Add task status patch endpoint`
- `Refactor task service update flow`
- `Add validation for login request`

Avoid:
- `changes`
- `updates`
- `fix stuff`
- overly long commit messages

## Workflow

When preparing a commit:
1. Check git status.
2. Review changed files and diff if needed.
3. Decide whether the changes are logically grouped.
4. Suggest a concise commit message.
5. If asked, stage and commit the changes.

## Output Expectations

When helpful, provide:
- short summary of what changed
- suggested commit message
- warning if the change looks too broad for one commit

## Final Rule

Optimize for clean Git history and reviewability, not clever wording.