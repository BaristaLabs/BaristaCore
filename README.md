BaristaCore
--------

#### Build Status

|               __Platform__    | __AppVeyor/Travis CI__ |
|:-----------------------------:|:----------------------:|
| __Windows (x64)__             | [![Build status](https://ci.appveyor.com/api/projects/status/4bk8y6id53ltv72m?svg=true)](https://ci.appveyor.com/project/Oceanswave/baristacore) [![Coverage Status](https://coveralls.io/repos/github/BaristaLabs/BaristaCore/badge.svg?branch=master)](https://coveralls.io/github/BaristaLabs/BaristaCore?branch=master)|
| __Ubuntu 14.04 (x64)__        | [![Build Status](https://travis-ci.org/BaristaLabs/BaristaCore.svg?branch=master)](https://travis-ci.org/BaristaLabs/BaristaCore) |
| __macOS 10.12.1 (x64)__       | [![Build Status](https://travis-ci.org/BaristaLabs/BaristaCore.svg?branch=master)](https://travis-ci.org/BaristaLabs/BaristaCore) |


> **06/28/2020** With the official word from Microsoft regarding ChakraCore being that "Microsoft will continue to provide security updates for Chakracore 1.11 until 9th March 2021 but do not intend to support it after that." This project is basically dead. Investigating possibilities of using V8 as a backing engine.

> Updated with ChakraCore 1.11.5

Provides a sandboxed JavaScript runtime natively to a .Net Standard 2.0 application on Windows, Linux and macOS.

Applications can expose custom modules and types written in .Net code to the runtime, making it useful for providing a rules engine or other scripted capabilities that interact with existing managed libraries.
     
The underlying JavaScript runtime is Chakra, the JavaScript engine that powers Microsoft Edge, allowing for a fully managed, latest standards compliant, performant and well sandboxed JavaScript-based scripting environment.

The BaristaCore package comes with an extensions library that enables hybrid .net functionality, such as on-the-fly transpilation between TypeScript and JavaScript, server-side rendering via React/React-Dom-Server and more.

Additionally, BaristaCore comes with a set of WebAPI middleware, allowing an application to provide a Functions-as-a-Service platform where scripts, stored as content, can be used to power web-based applications.

Embedding BaristaCore Within your own application
 ----------

BaristaCore is available on NuGet and can be simply added to any dotnet standard 2.0 application.
It can be found as a cross-platform .Net Standard 2.0 NuGet Package here:
https://www.nuget.org/packages/BaristaCore/

For in-depth instruction, please read [this wiki topic on embedding BaristaCore](https://github.com/BaristaLabs/BaristaCore/wiki/Embedding-BaristaCore-into-your-own-application)

## [Roadmap](https://github.com/BaristaLabs/BaristaCore/wiki/Roadmap)

For details on planned features and future direction please refer to the [Roadmap](https://github.com/BaristaLabs/BaristaCore/wiki/Roadmap).
