---
name: branch-manager
description: Create and validate Git branches for ProjectForge work using clear, consistent branch naming.
---

# Branch Manager

Use this skill when asked to create, validate, or prepare a Git branch for new work in ProjectForge.

## Purpose

Create a clean feature/fix/chore branch before implementation begins.

## Project Context

ProjectForge is a small .NET 10 Web API project. Keep branch names simple, readable, and PR-friendly.

## Rules

1. Inspect the current branch before creating a new one.
2. Check for uncommitted changes before branching.
3. Do not create a new branch if the user is already on an appropriate working branch unless asked.
4. Prefer branch names in one of these formats:
- `feature/<short-description>`
- `fix/<short-description>`
- `chore/<short-description>`

Examples:
- `feature/task-update-endpoint`
- `fix/login-error-handling`
- `chore/add-task-service-tests`

## Naming Guidance

- use lowercase
- use hyphens
- keep names short but meaningful
- avoid vague names like `stuff`, `changes`, `test-branch`

## Workflow

When asked to create a branch:
1. Check current branch.
2. Check working tree status.
3. If needed, create a new branch with a clear name.
4. Confirm the final branch name.

## Output Expectations

When helpful, provide:
- current branch
- whether a new branch was created
- the final branch name

## Final Rule

Prefer safe, clean Git hygiene.  
Do not switch branches blindly if there are uncommitted changes that could be disrupted.