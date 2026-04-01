---
name: pre-pr-review
description: Perform a lightweight pre-PR review for ProjectForge changes before commit, push, or PR creation.
---

# Pre-PR Review

Use this skill before creating a PR for ProjectForge.

## Purpose

Run a quick, practical review pass to catch obvious issues before the branch is pushed or turned into a PR.

## Project Context

ProjectForge is a .NET 10 Web API project with:
- REST endpoints
- EF Core
- PostgreSQL
- JWT auth
- role-based authorization
- layered architecture

This skill is a lightweight checkpoint, not a full audit.

## Review Priorities

Focus on:
1. obvious correctness issues
2. obvious security issues
3. changes that are too large or mixed in scope
4. missing auth or role checks on sensitive endpoints
5. poor API behavior such as wrong status codes or missing not-found handling
6. DTO/entity misuse
7. risky config or hardcoded values

## Workflow

When asked to do a pre-PR review:
1. Inspect current changes.
2. Flag only meaningful issues.
3. Keep feedback concise and actionable.
4. If useful, align with the `code-review` and `security-review` skills instead of duplicating their full behavior.

## Output Format

## Pre-PR Review Summary
Short summary of readiness for PR.

## Findings
For each finding include:
- Title
- Severity
- Why it matters
- Recommended fix

If there are no meaningful findings, say:
- No significant issues identified before PR creation.

## Final Rule

Keep this fast, practical, and high-value.  
Prefer a few useful findings over a long review.