# SpaceRockIT Festival — Domain Background & Operational Context

## Festival Overview
SpaceRockIT is a hybrid open-air IT conference and music festival in the Netherlands. Attendees participate in daytime informational and technical sessions in tents and attend live concerts in the evening.

## Operational Realities
1. **Flaky Open-Air Wi-Fi:** Festival network connectivity drops frequently. Clients will retry failed submissions, requiring idempotent handling.
2. **Attendee Privacy & GDPR Standards:**
   - Free-text comments submitted by attendees often inadvertently contain email addresses (e.g. *"Share your slides at alex.dev@enterprise.org"*).
   - Under European GDPR regulations, **email addresses must NEVER be logged in plain text or echoed in aggregate responses**.
   - All email patterns matching RFC 5322 must be sanitized to `[redacted-email]` prior to logging or storage.