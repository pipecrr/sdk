export function PoblateWatermark() {
    setTimeout(function () {
        var clientWidth = document.body.clientWidth;
        Array.from(document.querySelectorAll('.sdk-test-watermark')).forEach(function(el) {

            var text = el.dataset.watermark;
            var textLength = text.length;
            var repeatTimes = Math.floor(clientWidth / textLength * 10);

            el.dataset.watermark = (el.dataset.watermark + ' ').repeat(repeatTimes);
        });
    }, 500);
}

export function checkAndRenewToken(dotnethelper) {
    var timeout_seconds = 10;
    var token = localStorage.getItem('usertoken');
    if (token) {
        var tokenData = JSON.parse(atob(token.split('.')[1]));
        var now = new Date();
        var tokenExpiration = new Date(tokenData.exp * 1000);
        if (now > tokenExpiration) {
            dotnethelper.invokeMethodAsync("ShowLogin");
            timeout_seconds = 5;
        }else if (now > new Date(tokenExpiration - 60000 * 5)) {
            dotnethelper.invokeMethodAsync("RenewToken");
        }else{
            //check if exits a div with class sdk-modal-login
            var loginModal = document.querySelector('.sdk-modal-login');
            if (loginModal) {
                dotnethelper.invokeMethodAsync("HideLogin");
            }
        }
    }
    setTimeout(function () {
        checkAndRenewToken(dotnethelper);
    }, 1000 * timeout_seconds);
    
} 

export function InitSDK(dotnethelper){
    document.onmousemove = resetTimeDelay;
    document.onkeypress = resetTimeDelay;
    window.SDKDotNetHelper = dotnethelper;

    function resetTimeDelay() {
        //save the last time the user interacted with the page
        localStorage.setItem('lastInteraction', new Date().getTime());
    }

    //get openned tabs from local storage and add the current tab
    var n_tabs = localStorage.getItem('n_tabs');
    if (n_tabs == null) {
        n_tabs = 0;
    }
    n_tabs++;
    localStorage.setItem('n_tabs', n_tabs);

    // //generate a guid
    // var tab_guid = (function () {
    //     function s4() {
    //         return Math.floor((1 + Math.random()) * 0x10000)
    //             .toString(16)
    //             .substring(1);
    //     }
    //     return function () {
    //         return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
    //             s4() + '-' + s4() + s4() + s4();
    //     };
    // })();

    // //save the guid in session storage
    // sessionStorage.setItem('guid', tab_guid);
    checkAndRenewToken(dotnethelper);

    // dispatch event
    var event = new CustomEvent('sdkinit', { detail: { dotnethelper: dotnethelper } });
    document.dispatchEvent(event);
}

window.addEventListener('storage', function(event){
    if (event.key == 'usertoken') {
        if(event.newValue == null){
            window.location.href = '/login';
        }
        
        if(event.oldValue != event.newValue && window.location.pathname == '/login'){
            // reload page
            window.location.reload();
        }
    }
});

//onclose event
window.addEventListener('beforeunload', function (e) {
    //get openned tabs from local storage and remove the current tab
    var n_tabs = localStorage.getItem('n_tabs');
    if (n_tabs == null) {
        n_tabs = 0;
    }
    n_tabs--;
    localStorage.setItem('n_tabs', n_tabs);

    //if there are no more openned tabs, remove the last interaction time
    if (n_tabs == 0) {
        localStorage.removeItem('lastInteraction');
        localStorage.removeItem('n_tabs');
        //remove user token
        localStorage.removeItem('usertoken');
        
    }
});