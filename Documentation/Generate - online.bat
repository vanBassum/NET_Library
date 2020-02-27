@echo off

set PATH=%PATH%;C:\docfx\

:GenerateDoc
docfx docfx.json 

:OpenBrowser
start "" http://localhost:8081
cls
echo ****************************************
echo *** To stop the server, press enter. ***
echo ****************************************

:StartServer
docfx serve -p 8081 _site

cls
echo ****************************************
echo *** Press R to regenerate the site.  ***
echo ****************************************

@CHOICE /C:R
IF ERRORLEVEL 1 GOTO GenerateDoc

:End
