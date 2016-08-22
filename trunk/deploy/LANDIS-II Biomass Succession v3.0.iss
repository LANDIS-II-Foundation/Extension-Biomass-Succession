#define PackageName      "Biomass Succession"
#define PackageNameLong  "Biomass Succession Extension"
#define Version          "3.0"
#define ReleaseType      "official"
#define ReleaseNumber    "3"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"

[Files]

Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Extension.Succession.Biomass.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: docs\LANDIS-II Biomass Succession v3.0 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples\biomass-succession

#define BioSucc3 "Biomass Succession 3.0.txt"
Source: {#BioSucc3}; DestDir: {#LandisPlugInDir}

; Libraries
Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Library.Succession.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall
Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Library.BiomassCohorts.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall


[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"
Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Succession"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#BioSucc3}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove the entry for the plug-in
; Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Succession v3"" "; WorkingDir: {#LandisPlugInDir}
[Code]

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Code section) v3.iss"
//-----------------------------------------------------------------------------

function CurrentVersion_PostUninstall(currentVersion: TInstalledVersion): Integer;
begin
    Result := 0;
end;

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  CurrVers_PostUninstall := @CurrentVersion_PostUninstall
  Result := True
end;
