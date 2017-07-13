
REM Update script
@echo off
@set /A excount=excount+1
if %excount%==2 (
	@echo Waiting for the studio to close
	@timeout 5
	@echo Installing latest version of the studio...
	@7za.exe x %1 -o%2 -aoa
	@echo Installing latest version of the simulator...
	@powershell.exe -File %2\Simulator\Add-AppDevPackage.ps1
	@pause
) else (
	@echo Updating installer..
	@7za.exe e %1 -so Tools\extract.bat -aoa > extract_latest.bat
	@call extract_latest %*
)