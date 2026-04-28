[Setup]
AppName=سیستم حسابداری لاستیک فروشی
AppVersion=1.0.0
DefaultDirName={pf}\TireShopAccounting
DefaultGroupName=حسابداری لاستیک
OutputDir=Output
OutputBaseFilename=TireShopAccounting_Setup
Compression=lzma2
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\TireShopAccounting.exe
AppPublisher=Tire Shop Accounting
LicenseFile=
SetupIconFile=app.ico

[Languages]
Name: "farsi"; MessagesFile: "compiler:Languages\Farsi.isl"

[Tasks]
Name: "desktopicon"; Description: "ایجاد میانبر روی دسکتاپ"; GroupDescription: "میانبرها:"

[Files]
Source: "TireShopAccounting.UI\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\سیستم حسابداری"; Filename: "{app}\TireShopAccounting.exe"
Name: "{commondesktop}\سیستم حسابداری لاستیک"; Filename: "{app}\TireShopAccounting.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\TireShopAccounting.exe"; Description: "اجرای برنامه"; Flags: nowait postinstall skipifsilent
