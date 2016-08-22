#define PackageName      "Biomass Succession v2"
#define PackageNameLong  "Biomass Succession Extension"
#define Version          "2.2"
#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "5.1"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section).iss"


#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif

[Files]

; Cohort Libraries
Source: C:\Program Files\LANDIS-II\5.1\bin\Landis.Library.Cohorts.Biomass.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall

; Succession Library
Source: C:\Program Files\LANDIS-II\5.1\bin\Landis.Succession.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall

; Biomass Succession v2
Source: C:\Program Files\LANDIS-II\5.1\bin\Landis.Extension.Succession.Biomass_v2.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: docs\LANDIS-II Biomass Succession v2.2 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples\biomass-succession-v2

#define BioSucc2 "Biomass Succession 2.2.txt"
Source: {#BioSucc2}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"
Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Succession v2"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#BioSucc2}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove the entry for the plug-in
; Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Succession v2"" "; WorkingDir: {#LandisPlugInDir}

[Code]
#include AddBackslash(LandisDeployDir) + "package (Code section) v3.iss"




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
[Setup]
MergeDuplicateFiles=false
