import angular from 'angular';

import ResizeDirective from './directives/resize-directive';

export default angular
    .module('main.app.common', [])
    .directive('ncbResize', ResizeDirective)
    .name;