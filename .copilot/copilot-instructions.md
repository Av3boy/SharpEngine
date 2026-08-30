# Copilot/Agent Guidance (canonical: .github/copilot-instructions.md)

# Canonical location
- This file is the authoritative Copilot/automation guidance for this repository. Keep only this file as the canonical Copilot instruction source (remove any duplicate copies at repository root or elsewhere after review and approval).

# Commit and automation policy
- Agents MUST NOT create, amend, or push commits without explicit human approval. Present diffs and allow the user to explicitly approve (e.g., "Approve commit") before any commit is made.
- Include the required Co-authored-by trailer in commits made after explicit approval.

# Coding style and change guidance

- Favor composition over inheritance, and follow SOLID principles for maintainability and testability.
- When referencing system libraries (System.IO, System.Net.Http, etc.), prefer abstractions or interfaces to enable easier testing and mocking.

## Safetry
- Exception handling rule: do not use empty catch blocks. Every catch block must handle the exception appropriately; at minimum it must log the caught exception (including exception type and message). Swallowing exceptions silently is forbidden. Prefer catching specific exception types and rethrowing or wrapping as needed. For library code, include context in the log message to aid debugging.
- All code must be written in a thread-safe manner. Avoid static mutable state unless it is properly synchronized. Use locks, concurrent collections, or other synchronization primitives as appropriate.

- Update XML documentation for any new or changed public members.
- All public methods must validate their input parameters and throw appropriate exceptions (e.g., ArgumentNullException, ArgumentOutOfRangeException) when invalid arguments are provided.
- All public methods must document their behavior, including exceptions thrown, in XML documentation comments.

## Testing
- When making changes to the codebase, follow existing styles, naming, formatting, and conventions used in the repository. When in doubt, examine the dominant pattern in the affected area and mimic it, or ask the user for guidance.
- Ensure changes are unit testable. Add or update unit tests to cover new behavior and run existing tests locally when possible.

- All new code must be accompanied by unit tests that cover both typical and edge cases. Aim for high code coverage, but prioritize meaningful tests over coverage percentage. Use mocking frameworks as needed to isolate units under test.
- If another existing class is required that cannot be mocked or stubbed, refrain from adding unit tests and inform the user that the class cannot be tested in isolation. Ask the user for guidance on how to proceed (e.g., refactoring, adding interfaces, or using integration tests).

## Documentation and communication
- When adding new features or functionality, ensure that the design is modular and extensible. New features need to be documented in the repository's documentation (e.g., README.md, docs folder) and include usage examples where appropriate. If the ../SharpEngine-Docs/docs is present, prefer that. If that folder contains relevant documentation, update it accordingly.
- When modifying existing functionality, ensure that the changes do not break backward compatibility unless explicitly intended.

# Issue and repository inquiries
- When a user asks about an issue, the agent should check the repository's GitHub Issues list first and reference any relevant issues.
- If the agent cannot access GitHub Issues (network restrictions, lack of permissions, or API limits), clearly inform the user that Issues could not be checked and that the agent will fall back to local code inspection only after notifying the user.

# External access and transparency
- If any external service (GitHub, CI, package registries) cannot be reached from the execution environment, explicitly state which checks or steps were skipped and why.

# Safety and secrets
- Never create or commit secrets, credentials, or private tokens. If code seems to include secrets, raise an alert and ask the user how to proceed.

# PR / branch guidance (recommended)
- Use descriptive PR titles and reference related issue numbers. If this repository has branch naming conventions, follow them; otherwise, ask the user for preferred branch naming.

# What to present to the user
- For any set of changes: present a concise diff or list of changed files, a short explanation of the change's purpose, and which tests were run (or which could not be run due to limitations).
- Ask for explicit approval before making commits.

If you have additional preferences (tone, level of detail, or where this file should live), provide guidance and this file will be adjusted accordingly.