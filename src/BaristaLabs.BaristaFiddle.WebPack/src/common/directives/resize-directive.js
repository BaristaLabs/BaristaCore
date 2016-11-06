export function resize($window) {
    return function(scope, element) {
        var w = angular.element($window);
        w.bind('resize', function () {
            // trigger $digest when window is resized and call `update` function
            scope.$apply(update);
        });
        update(); // initial setup
        function update() {
            var height = w.height();
            var width = w.width();
            scope.windowHeight = height;
            scope.windowWidth = width;
            scope.style = function () {
                return {
                    'height': (height - 100) + 'px',
                    'width': (width - 100) + 'px'
                };
            };
        }
    };
};