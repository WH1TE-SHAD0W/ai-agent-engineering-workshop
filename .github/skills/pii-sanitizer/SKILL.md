---
name: pii-sanitizer
description: "Applies standard GDPR/PII email redaction patterns and sanitization algorithms to free-text user inputs, logging statements, and DTOs."
---

# Skill: PII Sanitizer (`pii-sanitizer`)

## Capabilities & Implementation Logic
- **Regex Standard:** Employs RFC 5322 regex: `[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}`.
- **Masking Strategy:** Replaces all email matches with `[redacted-email]`.
- **Implementation:** Injects C# sanitization filter: `Regex.Replace(input, pattern, "[redacted-email]")`.