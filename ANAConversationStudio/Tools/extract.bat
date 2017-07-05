@echo off
cls
timeout 5
7za.exe x %1 -o%2 -aoa
pause