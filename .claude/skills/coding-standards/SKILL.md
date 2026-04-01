---
name: coding-standards
description: Apply ProjectForge coding standards when creating or modifying code.
---

# Coding Standards

Use this skill when asked to create, extend, or refactor code in ProjectForge.

## Project Context

ProjectForge is a .NET 10 Web API using:
- REST endpoints
- EF Core
- PostgreSQL
- JWT authentication
- role-based authorization
- layered architecture

This is an intentionally incomplete learning project. Prefer clean, practical code over heavy abstractions.

## Core Rules

1. Follow the existing project structure.
- Keep controllers in the API layer
- Keep business logic in services
- Keep persistence concerns in infrastructure/data
- Keep entities and DTOs separate

2. Keep controllers thin.
- Controllers should handle HTTP concerns only
- Do not place business logic in controllers
- Delegate work to services

3. Use DTOs for API contracts.
- Do not expose entities directly from endpoints unless the project already does so intentionally
- Keep request and response shapes clear and minimal

4. Keep changes small and focused.
- Do not refactor unrelated areas
- Do not introduce broad architectural changes unless asked
- Prefer PR-sized work

5. Respect REST conventions.
- Use clear routes
- Return appropriate status codes
- Validate missing resources with 404
- Use 400 for invalid requests where appropriate

6. Write maintainable EF Core code.
- Prefer EF Core over raw SQL unless there is a strong reason
- Keep query logic readable
- Do not place DB logic in controllers

7. Keep auth and authorization consistent.
- Protect endpoints appropriately
- Do not bypass existing auth patterns
- Be careful with role-restricted behavior

8. Favor readability.
- Use clear naming
- Avoid clever or overly compact code
- Add TODOs only when they are genuinely useful

9. Preserve extensibility.
- Since the project is intentionally incomplete, leave clean extension points
- Do not over-finish features beyond the requested scope

## When Generating New Code

Prefer to:
- match existing naming and folder conventions
- create or update services before adding logic to controllers
- add DTOs when introducing new endpoint contracts
- keep code testable and easy to extend
- explain briefly what files were changed

## Avoid

Do not:
- introduce unnecessary patterns or abstractions
- mix business logic and HTTP concerns
- leak sensitive fields in DTOs
- add unrelated cleanup
- overengineer the solution

## Output Expectations

When asked to implement code:
- keep the solution scoped to the request
- mention assumptions briefly
- list changed files when helpful
- call out any TODOs or follow-up work only if relevant

## Final Rule

Write code that fits naturally into ProjectForge as if it were a small, reviewable PR.