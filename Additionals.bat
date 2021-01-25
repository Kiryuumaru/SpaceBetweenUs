@echo off
setlocal
cd /d %~dp0
echo Dowloading Additionals...
mkdir Additionals
curl "https://www.googleapis.com/drive/v3/files/17P_0FuXQUYFQOYGOUVQ86U-nSlbyAV-v?alt=media&key=AIzaSyByO2PHzFJB2y1iQlvrI66tDzbB409EQuc" -o "%cd%\Additionals\Additionals.zip"
echo Unzipping...
powershell -command "Expand-Archive -Force '%cd%\Additionals\Additionals.zip' '%cd%\Additionals\'"
echo Done.
pause >nul