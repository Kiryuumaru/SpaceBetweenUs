![Alturos.Yolo](Docs/logo-banner.png)

# SpaceBetweenUs

A state of the art real-time human social distance detection system for C# (Visual Studio). This project has CPU and GPU support, with GPU the detection works much faster. The primary goal of this project is an easy use of yolo, this package is available on nuget and you must only install two packages to start detection. In the background we use the Windows Yolo version of [AlexeyAB/darknet](https://github.com/AlexeyAB/darknet). Send an image path or the byte array to [yolo](https://github.com/pjreddie/darknet) and receive the position of the detected objects. Our project is meant to return the object-type and -position as processable data. This library supports [YoloV3 and YoloV2 Pre-Trained Datasets](#pre-trained-dataset)

## NuGet
Quick install Alturos.Yolo over [NuGet](https://www.nuget.org/packages/Alturos.Yolo)
```
PM> install-package Alturos.Yolo (C# wrapper and C++ dlls 28MB)
PM> install-package Alturos.YoloV2TinyVocData (YOLOv2-tiny Pre-Trained Dataset 56MB)
```

## Object Detection

![object detection result](doc/objectdetectionanimated.png)

## Performance
It is important to use GPU mode for fast object detection. It is also important not to instantiate the wrapper over and over again. A further optimization is to transfer the images as byte stream instead of passing a file path. GPU detection is usually 10 times faster!

## System requirements
- .NET Framework 4.6.1 or .NET standard 2.0
- [Microsoft Visual C++ Redistributable for Visual Studio 2015, 2017 und 2019 x64](https://aka.ms/vs/16/release/vc_redist.x64.exe)

### GPU requirements (optional)
It is important to use the mentioned version `10.2`

1) Install the latest Nvidia driver for your graphic device
2) [Install Nvidia CUDA Toolkit 10.2](https://developer.nvidia.com/cuda-downloads) (must be installed add a hardware driver for cuda support)
3) [Download Nvidia cuDNN v7.6.5 for CUDA 10.2](https://developer.nvidia.com/rdp/cudnn-download)
4) Copy the `cudnn64_7.dll` from the output directory of point 2. into the project folder.

## Build requirements
- Visual Studio 2019

## Benchmark / Performance
Average processing speed of test images bird1.png, bird2.png, car1.png, motorbike1.png

### CPU

Processor | YOLOv2-tiny | YOLOv3 | yolo9000 |
--- | --- | --- | --- | 
Intel i7 3770 | 260 ms | 2200 ms | - | 
Intel Xeon E5-1620 v3 | 207 ms | 4327 ms | - | 
Intel Xeon E3-1240 v6 | 182 ms | 3213 ms | - | 

### GPU

Graphic card | Single precision | Memory | Slot | YOLOv2-tiny | YOLOv3 | yolo9000 |
--- | --- | --- | --- | --- | --- | --- |
NVIDIA Quadro K420 | 300 GFLOPS | 2 GB | Single | - | - | - |
NVIDIA Quadro K620 | 768 GFLOPS | 2 GB | Single | - | - | - |
NVIDIA Quadro K1200 | 1151 GFLOPS | 4 GB | Single | - | - | - |
NVIDIA Quadro P400 | 599 GFLOPS | 2 GB | Single | - | - | - |
NVIDIA Quadro P600 | 1117 GFLOPS | 2 GB | Single | - | - | - |
NVIDIA Quadro P620 | 1386 GFLOPS | 2 GB | Single | - | - | - |
NVIDIA Quadro P1000 | 1862 GFLOPS | 4 GB | Single | - | - | - |
NVIDIA Quadro P2000 | 3011 GFLOPS | 5 GB | Single | - | - | - |
NVIDIA Quadro P4000 | 5304 GFLOPS | 8 GB | Single | - | - | - |
NVIDIA Quadro P5000 | 8873 GFLOPS | 16 GB | Dual | - | - | - |
NVIDIA GeForce GT 710 | 366 GFLOPS | 2 GB | Single | - | - | - |
NVIDIA GeForce GT 730 | 693 GFLOPS | 2-4 GB | Single | - | - | - |
NVIDIA GeForce GT 1030 | 1098 GFLOPS | 2 GB | Single | 40 ms | 160 ms | - |
NVIDIA GeForce GTX 1060 | 4372 GFLOPS | 6 GB | Dual | 25 ms | 100 ms | - |

## Troubleshooting

If you have some error like `DllNotFoundException` use [Dependencies](https://github.com/lucasg/Dependencies/releases) to check all references are available for `yolo_cpp_dll_gpu.dll`

If you have some error like `NotSupportedException` check if you use the latest Nvidia driver

### Debugging Tool for Nvidia Gpu

Check graphic device usage `"%PROGRAMFILES%\NVIDIA Corporation\NVSMI\nvidia-smi.exe"`

### Directory Structure

You should have this files in your program directory.

    .
    ├── Alturos.Yolo.dll              # C# yolo wrapper
    ├── yolo_cpp_dll_cpu.dll      # yolo runtime for cpu
    ├── yolo_cpp_dll_gpu.dll      # yolo runtime for gpu
    ├── cudnn64_7.dll             # required by yolo_cpp_dll_gpu (optional only required for gpu processig)
    ├── opencv_world340.dll       # required by yolo_cpp_dll_xxx (process image as byte data detect_mat)
    ├── pthreadGC2.dll            # required by yolo_cpp_dll_xxx (POSIX Threads)
    ├── pthreadVC2.dll            # required by yolo_cpp_dll_xxx (POSIX Threads)
    ├── msvcr100.dll              # required by pthread (POSIX Threads)