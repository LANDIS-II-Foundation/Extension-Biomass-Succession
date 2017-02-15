@echo off

rem  Run this script in the directory where it's located
pushd %~dp0
setlocal

set Script=%~nx0

rem  Set environment variables about specific library
set LibraryFileName=supportlibs.zip
set LibraryURL=https://github.com/LANDIS-II-Foundation/Support-Library-Dlls/archive/master.zip
set DownloadDir=bin\Debug
set LibraryPackage=%DownloadDir%\%LibraryFileName%

rem  download the specific library

set FileInPkg=supportlibs

call WinPkgTools\getPackage.cmd %LibraryUrl% %LibraryPackage% %FileInPkg%

move bin\Debug\Support-Library-Dlls-master\*.* bin\Debug

del /f /q bin\Debug\supportlibs.zip
rmdir /q bin\Debug\Support-Library-Dlls-master

:exitScript

popd

set ExitCode=0
if "%Action%" == "error" (
  set ExitCode=1
) else (
  set ExitCode=0
)
exit /b %ExitCode%

rem  ------------------------------------------------------------------------
:error

echo Error: %*
call :usage
set Action=error
goto :eof

rem  ------------------------------------------------------------------------
