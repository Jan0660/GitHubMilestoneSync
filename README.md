# GitHubMilestoneSync

Synchronizer for GitHub milestones and issue labels across multiple repositories.

## Usage

Your GitHub token is taken from the `GITHUB_TOKEN` environment variable and config file from `./config.json`.

`config.json` example:

```json
[
  {
    "repositories": [
      "revoltchat/revolt",
      "revoltchat/delta",
      "revoltchat/revite",
      "revoltchat/desktop",
      "revoltchat/self-hosted",
      "revoltchat/revolt.js",
      "revoltchat/vortex",
      "revoltchat/autumn",
      "revoltchat/january",
      "revoltchat/revolt.chat",
      "revoltchat/api",
      "revoltchat/translations",
      "revoltchat/themes",
      "revoltchat/documentation",
      "revoltchat/cli",
      "revoltchat/legal"
    ],
    "milestones": [
      {
        "title": "0.5.3: Backend Overhaul",
        "state": "open",
        "description": "Improve the state of the current backend."
      },
      {
        "title": "0.5.4: Moderation Tools",
        "state": "open",
        "description": "Add well-defined platform content and server moderation tools."
      },
      {
        "title": "0.5.5: Emoji Update",
        "state": "open",
        "description": "Add emojis, stickers and reactions."
      },
      {
        "title": "0.6.0: End-to-end Encrypted Chat",
        "state": "open",
        "description": "Implement secret chats for groups and DMs."
      },
      {
        "title": "(stalled) Native Bridges",
        "state": "open",
        "description": "Native bridges from Revolt to various providers such as Discord and Matrix."
      }
    ],
    "labels": [
      {
        "name": "bug",
        "color": "d73a4a",
        "description": "Something isn't working"
      }
    ]
  }
]
```