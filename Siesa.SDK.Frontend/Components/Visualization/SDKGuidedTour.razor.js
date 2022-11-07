var sdk_guided_tour_loaded = false;

function loadGuidedTour() {
    loadCss('/_content/Siesa.SDK.Frontend/vendor/annojs/anno.css');
    loadScript("/_content/Siesa.SDK.Frontend/vendor/jquery/jquery.min.js", true, function () {
        loadScript("/_content/Siesa.SDK.Frontend/vendor/annojs/jquery.scrollintoview.min.js", true);
        loadScript("/_content/Siesa.SDK.Frontend/vendor/annojs/anno.js", true, function () {

            if(window.sdk_guided_tours == null)
            {
                window.sdk_guided_tours = {};
            }

            var demo = new Anno([{
                target: '.SDKFormLayoutItem_E05003_City\\.State',
                content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam maximus augue a odio feugiat, at fermentum risus tristique. Pellentesque condimentum, nunc et iaculis imperdiet',
                buttons: [{
                    text: 'Cerrar',
                    className: 'anno-btn-low-importance',
                    click: function (anno, evt) {
                        anno.hide();
                        evt.preventDefault();
                    }
                },{
                    text: 'Siguiente',
                    click: function (anno, evt) {
                        anno.switchToChainNext();
                        //prevent default behavior
                        evt.preventDefault();
                    }
                }
                ]
            }, {
                target: '.SDKFormLayoutItem_E05003_City\\.Id',
                content: 'turpis lorem tincidunt elit, sed tincidunt velit quam a ex. Cras tincidunt ut erat sit amet pharetra. Sed vel luctus est, ut cursus purus.',
                buttons: [{
                    text: 'Cerrar',
                    className: 'anno-btn-low-importance',
                    click: function (anno, evt) {
                        anno.hide();
                        evt.preventDefault();
                    }
                },{
                    text: 'Siguiente',
                    click: function (anno, evt) {
                        anno.switchToChainNext();
                        evt.preventDefault();
                    }
                }
                ]
            }, {
                target: '.SDKFormLayoutItem_E05003_City\\.Status',
                content: 'Fusce quis sollicitudin enim. Integer in est quis ligula sodales convallis eu in lacus. Curabitur molestie diam vel velit tincidunt, dapibus elementum felis facilisis. ',
                buttons: [{
                    text: 'Finalizar',
                    click: function (anno, evt) {
                        anno.hide();
                        evt.preventDefault();
                    }
                }
                ]
            }]);

            sdk_guided_tours["demo"] = demo;

            //check if demo already shown and set cookie
            if(document.cookie.indexOf("demo_shown") == -1){
                sdk_guided_tours["demo"].show();
                document.cookie = "demo_shown=true";
            }

        });
    });
}

window.SDKStartGuidedTour =  function(tour_name){
    if (sdk_guided_tours[tour_name] != null) {
        sdk_guided_tours[tour_name].show();
    }
}

//check for event sdkinit
document.addEventListener('sdkinit', function (e) {
    //check if the guided tour is loaded
    if (!sdk_guided_tour_loaded) {
        loadGuidedTour();
    }
});

(() => {
    if (typeof (loadCss) != "undefined" && !sdk_guided_tour_loaded) {
        loadGuidedTour();
    }
})();