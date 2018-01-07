Portafilter
-----

This is a Azure Functions host of BaristaCore.

Caveat: Despite running perfectly locally, when deployed to Azure Functions (as of now) despite
having x64 enabled on the application configuration page, Azure Functions loads dotnet core in x86

wtf.

So you have to do the following in your app service via Kudu.

1. Download the x64 .net sdk into your d:\home\deployments\tools folder using curl.

Find the url of the sdk from
https://www.microsoft.com/net/download/windows

``` cmd
curl -O [.net sdkurl]
unzip [zipfilename]

```

2. Edit the web.config of the Functions to point to the x64 version of .net sdk.

This file is located in this path (version number might change)

D:\Program Files (x86)\SiteExtensions\Functions\2.0.11415-alpha>




.... nevermind this doesnt work.