; HR Management System - Inno Setup Script
; نظام إدارة الموارد البشرية

#define MyAppName "HR Management System"
#define MyAppNameArabic "نظام إدارة الموارد البشرية"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "HR Management"
#define MyAppURL "https://github.com/hr-management"
#define MyAppExeName "HRManagementSystem.exe"

[Setup]
AppId={{8F4D9A2C-1B3E-4C5F-A7D8-9E0F1B2C3D4E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=..\LICENSE.txt
InfoBeforeFile=..\INSTALL_INFO.txt
OutputDir=..\dist
OutputBaseFilename=HRManagementSystem_Setup_{#MyAppVersion}
SetupIconFile=..\src\HRManagementSystem\Resources\app.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
WizardResizable=no
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
ShowLanguageDialog=yes
CloseApplications=yes
RestartApplications=yes
UninstallRestartComputer=no
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=HR Management System Installer
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publish\*.pdb"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "..\Database\Schema\*.sql"; DestDir: "{app}\Database\Schema"; Flags: ignoreversion

[Dirs]
Name: "{app}\Data"; Permissions: users-full
Name: "{app}\logs"; Permissions: users-full
Name: "{app}\Backups"; Permissions: users-full

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec

[UninstallDelete]
Type: filesandordirs; Name: "{app}\logs"
Type: dirifempty; Name: "{app}"

[Code]
function InitializeSetup(): Boolean;
begin
  Result := True;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Create initial folders if needed
    ForceDirectories(ExpandConstant('{app}\Data'));
    ForceDirectories(ExpandConstant('{app}\logs'));
    ForceDirectories(ExpandConstant('{app}\Backups'));
  end;
end;

