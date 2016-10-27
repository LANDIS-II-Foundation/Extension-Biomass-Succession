#include GetEnv("LANDIS_SDK") + '\packaging\initialize.iss'

#define ExtInfoFile "Biomass Succession.txt"

#include LandisSDK + '\packaging\read-ext-info.iss'
#include LandisSDK + '\packaging\Landis-vars.iss'

[Setup]
#include LandisSDK + '\packaging\Setup-directives.iss'
LicenseFile={#LandisSDK}\licenses\LANDIS-II_Binary_license.rtf

[Files]
Source: {#LandisExtDir}\{#ExtensionAssembly}.dll; DestDir: {app}\bin\extensions; Flags: replacesameversion
Source: ..\src\lib\Landis.Library.Biomass-v1.dll; DestDir: {app}\bin\extensions; Flags: replacesameversion uninsneveruninstall
Source: ..\src\lib\Landis.Library.Succession.dll; DestDir: {app}\bin\extensions; Flags: replacesameversion uninsneveruninstall
Source: ..\src\lib\Landis.Library.BiomassCohorts.dll; DestDir: {app}\bin\extensions; Flags: replacesameversion uninsneveruninstall
Source: ..\src\lib\Landis.Library.AgeOnlyCohorts.dll; DestDir: {app}\bin\extensions; Flags: replacesameversion uninsneveruninstall
Source: ..\src\lib\Landis.Library.Cohorts.dll; DestDir: {app}\bin\extensions; Flags: replacesameversion uninsneveruninstall
Source: ..\src\lib\Landis.Library.Parameters-v1.dll; DestDir: {app}\bin\extensions; Flags: replacesameversion uninsneveruninstall


#define UserGuideSrc "LANDIS-II " + ExtensionName + " vX.Y User Guide.pdf"
#define UserGuide    StringChange(UserGuideSrc, "X.Y", MajorMinor)
Source: docs\{#UserGuide}; DestDir: {app}\docs; DestName: {#UserGuide}

Source: examples\*; DestDir: {app}\examples\{#ExtensionName}; Flags: recursesubdirs

#define ExtensionInfo  ExtensionName + " " + MajorMinor + ".txt"
Source: {#ExtInfoFile}; DestDir: {#LandisExtInfoDir}; DestName: {#ExtensionInfo}

[Run]
Filename: {#ExtAdminTool}; Parameters: "remove ""{#ExtensionName}"" "; WorkingDir: {#LandisExtInfoDir}
Filename: {#ExtAdminTool}; Parameters: "add ""{#ExtensionInfo}"" "; WorkingDir: {#LandisExtInfoDir}

[UninstallRun]
Filename: {#ExtAdminTool}; Parameters: "remove ""{#ExtensionName}"" "; WorkingDir: {#LandisExtInfoDir}

[Code]
#include LandisSDK + '\packaging\Pascal-code.iss'

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
