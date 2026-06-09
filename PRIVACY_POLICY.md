# TASHBOARD Privacy Policy

**Effective date: June 9, 2026**

TASHBOARD is a Chrome extension that helps users organize Tasks, local folder
bookmarks, links, and memos in the Chrome side panel. This Privacy Policy
explains how TASHBOARD handles user data.

## Data TASHBOARD Handles

TASHBOARD may handle the following data only to provide its user-facing
features:

- Task names, memos, tags, saved links, and local folder bookmark paths entered
  or selected by the user.
- The title and URL of the active page when the user explicitly chooses to add
  the current page.
- The URLs of tabs in the current Chrome window to indicate whether a saved
  link is currently open.
- Clipboard text only when the user explicitly clicks the add-from-clipboard
  button. TASHBOARD only attempts to use that text as a link.
- Theme preferences and Task state.
- Task configuration and backup files that the user explicitly saves, imports,
  exports, or downloads.

TASHBOARD does not collect personally identifiable information, health
information, financial information, authentication information, precise
location information, or personal communications.

## How Data Is Used and Stored

Task data is stored locally in the user's browser using browser storage and
IndexedDB. Exported JSON backups are saved only when requested by the user or
automatically before the user removes a Task to help prevent accidental data
loss.

The optional TASHBOARD Bridge communicates with the extension through Chrome
Native Messaging on the same computer. It is used only for user-requested local
folder selection, folder opening, Task configuration file saving and loading,
and linked-folder status checks.

The developer does not operate a server that receives or stores TASHBOARD Task
data. TASHBOARD does not use user data for advertising, profiling, credit
decisions, or purposes unrelated to its productivity features.

## External Resources

The extension may request font and icon style resources from Google Fonts and
cdnjs over HTTPS. These providers may receive ordinary network request
metadata, such as the user's IP address and browser information, according to
their own privacy policies. TASHBOARD does not send Task data, saved links,
memos, clipboard contents, or local folder paths to these providers.

## Data Sharing and Sale

TASHBOARD does not sell user data. TASHBOARD does not transfer user data to
third parties except for ordinary HTTPS requests needed to load the external
style resources described above. The developer does not allow humans to read
user Task data.

## Data Retention and Deletion

Locally stored data remains on the user's device until the user deletes a Task,
clears the extension's stored data, or uninstalls the extension. Users can
export Task data as JSON and delete Tasks from within TASHBOARD.

Files saved through the optional TASHBOARD Bridge remain in locations selected
by the user until the user deletes them.

## Security

TASHBOARD limits data access to the permissions required for its user-facing
features. Native Messaging communication with the optional Bridge occurs
locally on the user's computer. External style resources are requested over
HTTPS.

## Chrome Web Store Limited Use

TASHBOARD's use and transfer of information received from Google APIs adheres
to the Chrome Web Store User Data Policy, including the Limited Use
requirements.

## Changes to This Policy

This Privacy Policy may be updated when TASHBOARD's features or data practices
change. Updates will be published on this page with a revised effective date.

## Contact

For privacy questions or concerns, open an issue at:

https://github.com/rhymage/tashboard-bridge/issues
