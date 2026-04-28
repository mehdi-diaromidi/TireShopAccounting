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

[Code]
var
  IntroPage: TWizardPage;

function DotNet472OrNewerInstalled: Boolean;
var
  Release: Cardinal;
begin
  Result := False;
  if RegQueryDWordValue(HKLM64, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) then
    Result := Release >= 461814
  else if RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) then
    Result := Release >= 461814;
end;

function InitializeSetup: Boolean;
var
  Err: String;
begin
  if DotNet472OrNewerInstalled then
  begin
    Result := True;
    Exit;
  end;

  if MsgBox('برای کار کردن این برنامه، «Microsoft .NET Framework 4.7.2 یا بالاتر» روی ویندوز لازم است.' + #13#10 + #13#10 +
    'به نظر می‌رسد این مورد روی سیستم شما نیست یا خیلی قدیمی است.' + #13#10 + #13#10 +
    'برای دانلود از سایت رسمی مایکروسافت، «بله» را بزنید؛ بعد از نصب آن، دوباره همین برنامهٔ نصب را اجرا کنید.' + #13#10 + #13#10 +
    'اگر الان می‌خواهید خارج شوید «خیر» را بزنید.',
    mbConfirmation, MB_YESNO) = IDYES then
    ShellExecAsOriginalUser('open', 'https://dotnet.microsoft.com/download/dotnet-framework/net472', '', '', SW_SHOWNORMAL, ewNoWait, Err);

  Result := False;
end;

procedure InitializeWizard;
var
  IntroText: TNewMemo;
begin
  IntroPage := CreateCustomPage(
    wpWelcome,
    'راهنمای نصب',
    'نصب برنامه به صورت خودکار انجام می‌شود'
  );

  IntroText := TNewMemo.Create(WizardForm);
  IntroText.Parent := IntroPage.Surface;
  IntroText.Left := ScaleX(0);
  IntroText.Top := ScaleY(0);
  IntroText.Width := IntroPage.SurfaceWidth;
  IntroText.Height := IntroPage.SurfaceHeight;
  IntroText.ReadOnly := True;
  IntroText.BorderStyle := bsNone;
  IntroText.ScrollBars := ssVertical;
  IntroText.WordWrap := True;
  IntroText.WantReturns := True;
  IntroText.Text :=
    'مراحل نصب خیلی ساده است:' + #13#10 + #13#10 +
    '1) روی «ادامه» و سپس «نصب» کلیک کنید.' + #13#10 +
    '2) در پایان برنامه آماده استفاده است.' + #13#10 + #13#10 +
    'اگر پیام مربوط به .NET نمایش داده شد، ابتدا آن را نصب کنید و دوباره همین فایل نصب را اجرا کنید.';
end;
