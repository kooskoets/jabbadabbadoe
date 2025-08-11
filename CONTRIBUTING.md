# Contributing Guidelines

## Branch Strategy
- `main`: Production-ready code only.
- `develop`: Active development branch.
- Feature branches: `feature/<name>`
- Bugfix branches: `fix/<name>`

## Commit Messages
Follow [Conventional Commits](https://www.conventionalcommits.org/).

Examples:
- feat(bookings): add cancellation policy
- fix(auth): correct password hashing

## Pull Requests
1. Fork and branch from `develop`.
2. Commit using Conventional Commits.
3. Ensure all CI checks pass.
4. Submit PR to `develop`.
