title Making Repositories
echo off >nul
cd %~dp0 >nul

echo .
echo Cloning repos ...
echo ----------------------------------------------
if exist "Alturos.Yolo\.git" (
echo Already cloned
) else (
git clone https://https://github.com/Kiryuumaru/Alturos.Yolo
)
cd Alturos.Yolo
git submodule update --init
cd..
echo ----------------------------------------------
title Done
echo Done...
pause >nul