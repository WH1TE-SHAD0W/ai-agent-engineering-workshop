#!/usr/bin/env bash
# preToolUse gate. Reads the tool call on stdin, denies the ones we never want run.
#
# Contract: print nothing and exit 0 to allow. Print a permissionDecision to deny.
# Keep this under 5 seconds — see the warning about timeouts below.

set -uo pipefail

INPUT=$(cat)
CMD=$(printf '%s' "$INPUT" | jq -r '.toolArgs.command // empty' 2>/dev/null || true)

[ -z "$CMD" ] && exit 0

DENY_PATTERN='rm[[:space:]]+-rf|git[[:space:]]+push[[:space:]]+--force|git[[:space:]]+clean[[:space:]]+-fd([[:space:]]|$)|DROP[[:space:]]+TABLE|curl[^|]*\|[[:space:]]*(ba)?sh'

if printf '%s' "$CMD" | grep -qiE "$DENY_PATTERN"; then
  printf '%s' '{"permissionDecision":"deny","permissionDecisionReason":"Blocked by repository policy: destructive or unreviewed-execution command."}'
  exit 0
fi

exit 0