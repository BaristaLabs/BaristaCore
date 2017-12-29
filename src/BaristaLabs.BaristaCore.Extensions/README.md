BaristaLabs.BaristaCore.Extensions
--------

Provides additional functionality to the base BaristaCore

##### Module Loaders

 - WebResourceModuleLoader: Allows web resources accessible via http to be used.

##### Modules

 - ```barista-blob``` - Provides a implementation of the Blob object.
 - ```barista-console``` - Provides console.log functionality that writes to the currently configured log.
 - ```barista-fetch``` - Provides a fetch implementation
 - ```barista-handlebars``` - Provides a managed handlebars implementation
 - ```barista-react``` - Allows SSR via React
 - ```barista-mailkit``` - Adds the ability to send SMTP messages
 - ```barista-typescript``` - Adds the ability to transpile via TypeScript

##### Context Extensions

 - ExecuteTypeScriptModule - Adds the ability to automatically transpile and execute TypeScript