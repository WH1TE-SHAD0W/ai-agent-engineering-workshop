---
applyTo: "src/SpaceRockIT.Reviews.Api/**"
---

# Review Module Guardrails

When modifying or generating code within `src/SpaceRockIT.Reviews.Api/`:

1. **Rating Validation:**  
   Ensure all ratings are validated to the `1`–`5` range (inclusive). Return HTTP `400 Bad Request` for out-of-range ratings.

2. **PII Sanitization:**  
   Attendee comments must be sanitized for email patterns prior to logging or echoing in aggregate endpoints using the `/pii-sanitizer` skill.

3. **Testing Obligation:**  
   Every logic change in this module requires at least one automated xUnit test in `tests/SpaceRockIT.Reviews.Api.Tests/` using synthetic test fixtures.

4. **Observability:**  
   Log every accepted review with `ILogger` at `Information` level, including the workshop identifier, attendee identifier, rating, and the sanitized comment. Always log the redacted comment, never the raw input.