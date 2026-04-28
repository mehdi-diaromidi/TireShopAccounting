; نصب‌کنندهٔ ویندوز — Inno Setup 6 (ترجیحاً 6.5+) — زبان فارسی و راست‌به‌چپ
; پیش‌نیاز بیلد: TireShopAccounting.UI\bin\Release

#define MyAppName "حسابداری لاستیک‌فروشی"
#define MyAppNameShort "TireShopAccounting"
#define MyAppVersion "1.0.0"
#define MyAppExeName "TireShopAccounting.exe"
#define MyAppPublisher "TireShopAccounting"

#define SrcRelease "..\\TireShopAccounting.UI\\bin\\Release"

[Setup]
AppId={{E7B4A2C1-8F3D-4E2A-9B1C-5D6E7F8A9B0C}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={localappdata}\Programs\{#MyAppNameShort}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=..\dist
OutputBaseFilename={#MyAppNameShort}_Setup_{#MyAppVersion}
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma2/max
SolidCompression=yes
PrivilegesRequired=lowest
WizardStyle=modern
ShowLanguageDialog=no
DisableWelcomePage=no
MinVersion=6.1sp1
VersionInfoVersion=1.0.0.0
VersionInfoCompany={#MyAppPublisher}
VersionInfoProductName={#MyAppName}
CloseApplications=no

[Languages]
Name: "farsi"; MessagesFile: "Languages\Farsi.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "{#SrcRelease}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent
