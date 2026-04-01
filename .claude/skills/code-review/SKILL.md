---
name: code-review
description: Review ProjectForge changes for correctness, maintainability, and alignment with the project's .NET 10 Web API architecture.
---

# Code Review

Use this skill when asked to review code, a branch, a diff, or a PR for general code quality in ProjectForge.

## Project Context

ProjectForge is a .NET 10 Web API using:
- REST endpoints
- EF Core
- PostgreSQL
- JWT authentication
- role-based authorization
- layered architecture

This is an intentionally incomplete learning project. Prefer practical feedback that fits the current codebase over big redesigns.

## Focus Areas

Prioritize findings in these areas:

1. Correctness
- broken logic
- missing null/exists checks
- bad state handling
- incorrect REST behavior
- missing async/await where needed
- inconsistent DTO mapping

2. Architecture
- controllers doing business logic
- services bypassed unnecessarily
- leaking infrastructure concerns into API layer
- poor separation of concerns
- changes inconsistent with existing project structure

3. API Design
- incorrect status codes
- inconsistent routes
- poor request/response DTO usage
- returning entities directly when DTOs should be used
- unclear or unsafe endpoint contracts

4. Data Access
- inefficient or incorrect EF Core usage
- missing includes when clearly needed
- unnecessary raw SQL
- persistence logic in the wrong layer
- missing existence checks before update/delete behavior

5. Maintainability
- duplicated logic
- confusing naming
- hardcoded values that should be centralized
- difficult-to-follow flow
- missing TODOs where future work is obvious

6. Testing Readiness
- code structure that is hard to test
- tightly coupled logic
- side effects mixed into unrelated code
- missing seams for future unit tests

## Project-Specific Checks

Pay extra attention to:
- consistency between controllers, services, DTOs, and entities
- auth-related changes affecting endpoint behavior
- whether create/update logic stays in services
- whether API responses stay clean and predictable
- whether intentionally incomplete parts remain clean extension points

## What Not To Do

Do not:
- focus on formatting nitpicks
- rewrite the whole architecture
- suggest patterns that are too heavy for this project
- report vague issues without evidence
- turn the review into a security-only review

If you find a security issue, mention it briefly, but keep this review focused on overall code quality.

## Severity

Use:
- High: likely bug, broken behavior, major design issue, or clearly unsafe implementation
- Medium: meaningful maintainability or correctness issue
- Low: small improvement or cleanup suggestion

## Output Format

## Code Review Summary
Short summary of the overall code quality of the reviewed changes.

## Findings
For each finding include:
- Title
- Severity
- Why it matters
- Evidence
- Recommended fix

If there are no meaningful findings, say:
- No significant code quality issues identified in the reviewed changes.

## Follow-up Suggestions
Only include a few focused next steps if useful.

## Final Rule

Be concise, practical, and evidence-based.  
Prefer a small number of high-value findings over many low-value comments.