BaristaCore
--------

#### Build Status

|                               | __Release__ |
|:-----------------------------:|:-----------:|
| __Windows (x64)__             | [![Build status](https://ci.appveyor.com/api/projects/status/4bk8y6id53ltv72m?svg=true)](https://ci.appveyor.com/project/Oceanswave/baristacore) [![Coverage Status](https://coveralls.io/repos/github/BaristaLabs/BaristaCore/badge.svg?branch=master)](https://coveralls.io/github/BaristaLabs/BaristaCore?branch=master)|
| __Ubuntu 14.04 (x64)__        | [![Build Status](https://travis-ci.org/BaristaLabs/BaristaCore.svg?branch=master)](https://travis-ci.org/BaristaLabs/BaristaCore) |
| __macOS 10.12.1 (x64)__       | [![Build Status](https://travis-ci.org/BaristaLabs/BaristaCore.svg?branch=master)](https://travis-ci.org/BaristaLabs/BaristaCore) |


> **12/7/2017** *BaristaCore is currently in active development. The functionality described below indicates the design goals of BaristaCore and may not be all currently implemented.*

> Updated with ChakraCore 1.7.4

BaristaCore is an open-source cross-platform sandbox to run code with a http call.

BaristaCore improves upon barista-sharepoint by decoupling SharePoint from the equation, allowing Barista to execute as part of any .Net WebAPI middleware

In addition, Barista has been rewritten to use the ChakraCore Engine within .Net Core to allow Barista to execute in cross-platform environments.

To summarize, BaristaCore provides:

 - Barista as generic WebAPI Middleware
 - .Net Core Support
 - ChakraCore as the JavaScript Engine
 - Cross-Platform (Windows/OSx/Linux) Support
 - New features and services.
 - Better documentation and distribution.


 Other players in the serverless service app space that have sprung up in the past few years include:


 - AWS Lambda
 - Azure Functions
 - Auth0 WebTask
 - iron.io
 - Hyperdev

 However, each of the solutions above are closed-source cloud implementations. BaristaCore lets you have the same types of flexability as the above, but within your own datacenter.

Roadmap
----------

Core Functionality:
  - [X] Barista as WebAPI Middleware
    - [ ] Support configuring endpoint url
    - [ ] Configurable output formatters
    - [ ] Configurable Source File Providers (ISourceFileProvider)
	- [ ] Auto-Response marshalling based on result object type (ArrayBuffer/DataView = raw response)
	- [ ] Re-implementation of X- Header behavior (instancing, language, code location)
  - [X] .Net Core Support
  - [X] Use ChakraCore as the JavaScript Engine
    - [X] Enables ECMAScript 6+ support (classes, arrow functions, etc), and beyond.
    - [X] ES6 Promises
    - [X] ES6 Modules
    - [X] More performant script execution
	- [X] T4 Template driven generation of P/Invoke classes
	- [X] Full unit test suite surrounding managed/unmanaged interop layer
    - [ ] Debugging Support
	- [ ] Time-Travel Debugging Support
  - [X] Cross-Platform Support
    - [X] Provide Pre-built ChakraCore.dll binaries for 3 platforms
    - [X] Provide an automatic way for the .dll to be specified.
  - [ ] High-Level Object Model around ChakraCore
  - [ ] Automatic TypeScript transpilation
  - [ ] Built-in Swagger/Apiary API mocks, ability to implement.
  - [ ] Provide for WASM support, allowing *any* front-end language to be used (so long as there's a WASM compiler)
	- [ ] Have Fiddle provide automatic WASM compilation for a variety of languages.

New Services:
 - [ ] Define 'apps' as logical containers of files
   - [ ] Bundles can be enabled/disabled on a per-app basis
   - [ ] Source file providers can be defined and configured on a per-app basis
   - [ ] Define environment-variables per app.
   - [ ] Apps automatically include metrics.
   - [ ] More app-centric services, index, secure key store, etc...
 - [ ] Built-In Scheduling via Quartz.Net
 - [ ] Built-In Lucene.Net 4.8 Support (Lucene .Net Core Support forthcoming)
 - [ ] Global Bundle Management Service
 - [ ] Support multi-tenancy in cloud environments. (https://xxx/{user}/{app}/???)

Bundle re-implmentation
 - [ ] http/request
 - [ ] SharePoint
 - [ ] Lucene
 - [ ] Azure
 - [ ] Scraping
 - [ ] PDF Generation
 - [ ] Local Automation
 - [ ] Legacy Bundles (for compatibility)
 - [ ] ...

New extensibility support:
 - [ ] ISourceFileProvider Allows implementations of external stores to be used
   - require(...) now supports webpack-like syntax to use external file stores for instance:
   ``` javascript
   const foo = require('github:myrepo!'/foo.js);
   ```
 - [ ] API Wrappers generated via T4 templates to minimize LoE when exposing existing APIs to BaristaCore

Fiddle Improvements:
 - [ ] Provide Fiddle as WebAPI Middleware
 - [ ] Monaco Editor based
 - [ ] Develop as a PWA for offline/local dev.
 - [ ] Use webpack for bundling/minification
   - [ ] Seperate node.js based environment for actually developing BaristaFiddle
 - [ ] Support UI/UX around new services
   - [ ] Apps
   - [ ] Scheduling/Cron
   - [ ] ...
 - [ ] TypeScript syntax support
 - [ ] Provide better 'save' support via SourceFileProviders
 - [ ] Debugging support
 - [ ] Time-Travel Debugging Support

BaristaServer-SharePoint
 - [ ] SP2016 Deployment Scripts
 - [ ] Less dependency on SharePoint-level services (Farm Property Bag, et. al.)
 - [ ] Promote Barista Search Index (Lucene.Net 4.8) to be a stand-alone service application

Documentation
 - [x] Barista Site (baristalabs.io)
   - [ ] Update with BaristaCore
 - [ ] BaristaCore Docs (Github Wiki)

Distribution (BaristaServer)
 - [ ] Chocolatey
 - [ ] Brew
 - [ ] Apt-Get

Continuous Integration
 - [X] Automated Cross-Platform builds w/ Unit Tests
  - [X] Windows
  - [X] Linux
  - [X] macOS
 - [ ] Automated Releases
  - [ ] Chocolatey
  - [ ] Apt-Get
  - [ ] Brew
