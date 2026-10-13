#define MyAppName "Task Tracker Bot"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "HBT Workspace"
#define MyAppExeName "TaskTrackerBot.exe"

[Setup]
AppId={{A2F0D38F-38C5-49A7-A7A4-2E8D8F5D96E1}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\HBT Workspace\Task Tracker Bot
DefaultGroupName=HBT Workspace\Task Tracker Bot
AllowNoIcons=no
OutputDir=Installer
OutputBaseFilename=TaskTrackerBotSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
SetupIconFile=E:\HBT Workspace\Internal Project\Published\TaskTracker\TaskTrackerBot\Assets\daily-calendar.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked
Name: "startup"; Description: "Launch Task Tracker Bot automatically when Windows starts"; GroupDescription: "Additional options:"; Flags: unchecked

[Files]
Source: "E:\HBT Workspace\Internal Project\Published\TaskTracker\TaskTrackerBot\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Task Tracker Bot"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\Task Tracker Bot"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userstartup}\Task Tracker Bot"; Filename: "{app}\{#MyAppExeName}"; Tasks: startup

[Run]
Filename: "{app}\{#MyAppExeName}";Description: "Launch Task Tracker Bot";Flags: nowait postinstall skipifsilent