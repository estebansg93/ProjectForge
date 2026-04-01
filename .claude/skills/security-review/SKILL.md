---
name: security-review
description: Review ProjectForge changes for security issues in the .NET 10 Web API, EF Core, PostgreSQL, and JWT auth stack.
---

# Security Review

Use this skill when asked to review code, a branch, a diff, or a PR for security issues in ProjectForge.

## Project Context

ProjectForge is a .NET 10 Web API using:
- REST endpoints
- EF Core
- PostgreSQL
- JWT authentication
- role-based authorization
- layered architecture

This is an intentionally incomplete learning project, so focus on real security risks, not general code quality.

## Focus Areas

Prioritize findings in these areas:

1. Authentication
- plaintext password storage or comparison
- weak password hashing
- insecure login flow
- user enumeration
- hardcoded JWT secrets
- weak token validation or expiration handling

2. Authorization
- missing `[Authorize]`
- missing role restrictions
- admin actions exposed to non-admins
- privilege escalation paths
- service logic that assumes authorization without enforcing it

3. Input and API Safety
- missing validation
- dangerous trust in client input
- mass assignment risks
- unsafe state-changing endpoints
- overly permissive CORS if configured

4. Data Exposure
- password hashes or secrets returned in responses
- DTOs exposing internal fields
- detailed internal errors exposed externally

5. Database Safety
- unsafe raw SQL
- query string concatenation
- bypassing EF Core protections unsafely
- data access patterns that ignore authorization boundaries

6. Configuration and Secrets
- hardcoded credentials
- secrets committed to code or config
- insecure defaults that should be called out

## Project-Specific Checks

Pay extra attention to:
- auth/login endpoints
- JWT config
- `[Authorize]` and role enforcement on create/update/delete endpoints
- cross-project access issues
- DTOs exposing sensitive fields
- insecure seeded users or credentials

## What Not To Do

Do not:
- do a general style review
- focus on naming or formatting
- invent vulnerabilities without evidence
- recommend large rewrites unless the risk is serious

Be specific and evidence-based.

## Severity

Use:
- High: unauthorized access, privilege escalation, credential/token compromise, sensitive data exposure
- Medium: meaningful weakness with realistic abuse potential
- Low: smaller risk or hardening opportunity

## Output Format

## Security Review Summary
Short summary of the overall security posture of the reviewed changes.

## Findings
For each finding include:
- Title
- Severity
- Why it matters
- Evidence
- Recommended fix

If there are no meaningful findings, say:
- No significant security issues identified in the reviewed changes.

## Follow-up Checks
Only include if useful.

## Final Rule

Report only plausible, grounded security issues.  
If something is uncertain, say so clearly.