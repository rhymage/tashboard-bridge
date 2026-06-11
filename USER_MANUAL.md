# TASHBOARD User Manual

TASHBOARD is a task dashboard that keeps local folder bookmarks, useful links,
and quick memos together in Chrome's side panel.

## Contents

- [Getting Started](#getting-started)
- [Tasks](#tasks)
- [Folders and TASHBOARD Bridge](#folders-and-tashboard-bridge)
- [Links](#links)
- [Memos](#memos)
- [Search, Tags, and Pins](#search-tags-and-pins)
- [Themes and Shortcuts](#themes-and-shortcuts)
- [Backup, Restore, and Removal](#backup-restore-and-removal)
- [Troubleshooting](#troubleshooting)
- [Privacy and Support](#privacy-and-support)

## Getting Started

1. Install TASHBOARD from the Chrome Web Store.
2. Click the TASHBOARD icon in the Chrome toolbar.
3. Click the large **+** button to create your first Task.
4. Give the Task a clear title, then add folders, links, and memos.

TASHBOARD stores its data locally in Chrome. A Task is an independent workspace,
so you can use separate Tasks for projects, clients, research topics, or daily
work.

## Tasks

### Create a Task

Click the large **+** button beside the Task selector. You can create a blank
Task or create one from the current Chrome page when that option is available.

### Switch Tasks

Open the Task selector at the top of the panel and choose a Task. You can also
use **Shift + 1** through **Shift + 9** to switch to one of the first nine Tasks.

### Open a Task in a New Window

Open the Task selector and use the new-window button beside a Task. TASHBOARD
will open that Task in a separate Chrome window.

### Manage Tasks

Open the Task selector and choose **Manage Tasks** to rename, reorder, or manage
your Task list.

## Folders and TASHBOARD Bridge

Chrome extensions cannot directly select and open Windows folders. TASHBOARD
uses the optional TASHBOARD Bridge for these local folder features.

### Install the Bridge

1. Click the **BRIDGE** button in the Folders section.
2. Download
   [TashboardBridgeSetup.exe](https://github.com/rhymage/tashboard-bridge/releases/latest/download/TashboardBridgeSetup.exe).
3. Run the installer once.
4. Return to TASHBOARD. The **BRIDGE** button will change to the add-folder
   button when the connection is available.

The Bridge does not run continuously at Windows startup. Chrome starts it only
when TASHBOARD requests a local folder action.

### Add and Open Folders

Click the add-folder button and select a Windows folder. Click a saved folder
bookmark to open it in File Explorer. Use the pin and delete controls on each
folder bookmark to organize the list.

### Uninstall the Bridge

Open **Windows Settings > Apps > Installed apps**, find **TASHBOARD Bridge**,
and choose **Uninstall**.

## Links

### Add the Current Page

Click the add-current-page button in the Links section. TASHBOARD saves the
active page title and URL. Duplicate links are not added.

### Add a Link from the Clipboard

Copy a web address, then click the clipboard-link button. TASHBOARD validates
the address before adding it.

### Open, Edit, and Delete Links

Click a saved link to open it. If the same page is already open in the current
Chrome window, TASHBOARD activates that tab instead of creating a duplicate.
Use the controls beside a link to pin, edit, tag, or delete it.

## Memos

Enter text in the memo field and click the save button. You can also press
**Enter** to save a quick memo; use **Shift + Enter** when you need a line
break. Use the controls beside a memo to pin, edit, tag, or delete it.

## Search, Tags, and Pins

### Search

Use the search field at the top of the panel to search folders, links, and
memos in the current Task. Press **Escape** or click the clear button to reset
the search.

### Tags

Use the settings button in the Links or Memos section to manage tags. Click a
tag filter to show matching items. Pinned items remain easy to reach.

### Pins

Pin important folders, links, and memos to keep them at the top of their
sections.

## Themes and Shortcuts

Use the theme switcher near the top-right corner to choose Light or Dark mode.

Available keyboard shortcuts:

| Shortcut | Action |
| --- | --- |
| `Shift + 1` through `Shift + 9` | Switch Tasks |
| `Ctrl + Z` | Undo the last change |
| `Ctrl + Y` or `Ctrl + Shift + Z` | Redo the last undone change |
| `Enter` | Confirm most dialogs or save a quick memo |
| `Escape` | Close a dialog, close a popover, or clear search |

## Backup, Restore, and Removal

### Export a Task

Select the Task and click **EXPORT**. TASHBOARD creates a JSON backup containing
the Task's folders, links, memos, tags, and settings.

### Import a Task

Click **IMPORT** and select a TASHBOARD JSON backup file. Imported data is added
as a Task.

### Remove a Task

Click **REMOVE TASK FROM LIST** and confirm the removal. Before deleting the
Task, TASHBOARD automatically starts downloading a JSON backup to Chrome's
default download folder. If the backup download cannot start, the Task is not
deleted.

Keep exported JSON files private when they contain sensitive folder paths,
links, or memos.

## Troubleshooting

### The BRIDGE Button Does Not Change

- Confirm that **TASHBOARD Bridge** appears in Windows Installed apps.
- Close and reopen the TASHBOARD side panel.
- Restart Chrome after installing the Bridge.
- If necessary, download and run the latest Bridge installer again.

### A Folder Does Not Open

- Confirm that the folder still exists and that your Windows account can access
  it.
- Confirm that the TASHBOARD Bridge is installed.
- Remove the bookmark and add the folder again if its path changed.

### Import Does Not Work

Use a JSON backup created by TASHBOARD. Files with an unsupported or damaged
structure cannot be imported.

### Changes Were Made by Mistake

Use **Ctrl + Z** to undo recent changes in the current Task. For larger recovery
needs, import a previously exported JSON backup.

## Privacy and Support

TASHBOARD stores Task data locally and does not operate a server that receives
your Task data. Read the full [Privacy Policy](PRIVACY_POLICY.md) for details.

For questions, bug reports, or support, open an issue in the
[TASHBOARD Bridge repository](https://github.com/rhymage/tashboard-bridge/issues).
