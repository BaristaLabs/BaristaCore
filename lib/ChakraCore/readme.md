This folder contains pre-built ChakraCore libraries for macOS, Ubuntu and Windows

chakracore.dll - Windows 10, x64, Release - 1.7.5
libChakraCore.dylib - macOS Sierra, amd64, Release - 1.7.5
libChakraCore.so - Ubuntu 16.04.1, amd64, Release - 1.7.5


These pre-build binaries are only included for convenience as ChakraCore is a relatively big build and these cover the majority of platforms.

For other platforms, build [ChakraCore](https://github.com/microsoft/chakracore/) yourself and overwrite the dynamic libary for the target platform in the output folder.


Windows Build:

> Note - with VS2017 download and install https://developer.microsoft.com/en-us/windows/downloads/windows-8-1-sdk separately - for some reason the version that VS2017 installs doesn't install cor.h..??

```
msbuild /m /p:Platform=x64 /p:Configuration=Release /p:RuntimeLib=static_library Build\Chakra.Core.sln
```
