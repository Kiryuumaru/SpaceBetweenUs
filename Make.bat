title Making Repositories
echo off >nul
setlocal >nul
cd /d %~dp0 >nul
cls
echo .
echo Cloning repos ...
echo ----------------------------------------------
if exist "Alturos.Yolo\.git" (
echo Already cloned
) else (
git clone https://github.com/Kiryuumaru/Alturos.Yolo
)
cd Alturos.Yolo
git submodule update --init
cd..

title Downloading Large Files
echo ----------------------------------------------
echo .
echo Dowloading additionals large files ...
echo ----------------------------------------------
mkdir Additionals
curl "https://www.googleapis.com/drive/v3/files/1EEIjiuMSmObt37HdKFRpfiQ55Y0yNOtI?alt=media&key=AIzaSyDefGXOc8Ud3eFRAm-53SLbpAFxBqtbfCc" -o "%cd%\Additionals\Additionals.zip"
echo Unzipping...
powershell -command "Expand-Archive -Force '%cd%\Additionals\Additionals.zip' '%cd%\Additionals\'"
echo Moving files...
move "%cd%\Additionals\coco.names" "%cd%\SpaceBetweenUs\Assets\coco.names"
move "%cd%\Additionals\yolov3.cfg" "%cd%\SpaceBetweenUs\Assets\yolov3.cfg"
move "%cd%\Additionals\yolov3.weights" "%cd%\SpaceBetweenUs\Assets\yolov3.weights"
move "%cd%\Additionals\cudnn64_7.dll" "%cd%\SpaceBetweenUs\cudnn64_7.dll"
echo ----------------------------------------------
title Done
echo Done...
pause >nul