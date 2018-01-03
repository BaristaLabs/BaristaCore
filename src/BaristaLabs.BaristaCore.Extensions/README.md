BaristaLabs.BaristaCore.Extensions
--------

Provides commonly used modules, module loaders, and more to BaristaCore

##### Module Loaders

 - WebResourceModuleLoader: Allows web resources accessible via http to be used.

##### Modules

 - ```barista-blob``` - Provides a implementation of the Blob object.
 - ```barista-console``` - Provides console.log functionality that writes to the currently configured log.
 - ```barista-fetch``` - Provides a fetch implementation
 - ```barista-mailkit``` - Adds the ability to send SMTP messages
 - ```barista-xml2js``` - Converts Json to and from Xml

 - ```handlebars``` - Exposes the excellent Handlebars library
 - ```lodash``` - Exposes lodash
 - ```moment``` - Exposes moment
 - ```react``` - Allows SSR via React
 - ```react-dom-server``` - Allows SSR via React
 - ```typescript``` - Exposes the typescript library for transpiling Typescript to JavaScript
 - ```uuid``` - Exposes the uuid library for generating uuids

##### Context Extensions

 - ExecuteTypeScriptModule - Adds the ability to automatically transpile and execute TypeScript