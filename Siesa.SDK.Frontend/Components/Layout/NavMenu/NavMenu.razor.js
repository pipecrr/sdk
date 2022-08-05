function HideSDKMenu() {
    var menu = document.getElementById("menu");
    menu.checked = false;
}

function ShowSDKMenu() {
    var menu = document.getElementById("menu");
    menu.checked = true;
}


function CheckResolution() {
    if (window.innerWidth < 576) {
        HideSDKMenu();
    }
}

window.addEventListener('resize', function () {
    CheckResolution();
});

window.addEventListener('locationchange', function () {
    CheckResolution();
});


(() => {
    CheckResolution();
    let oldPushState = history.pushState;
    history.pushState = function pushState() {
        let ret = oldPushState.apply(this, arguments);
        window.dispatchEvent(new Event('pushstate'));
        window.dispatchEvent(new Event('locationchange'));
        return ret;
    };

    let oldReplaceState = history.replaceState;
    history.replaceState = function replaceState() {
        let ret = oldReplaceState.apply(this, arguments);
        window.dispatchEvent(new Event('replacestate'));
        window.dispatchEvent(new Event('locationchange'));
        return ret;
    };

    window.addEventListener('popstate', () => {
        window.dispatchEvent(new Event('locationchange'));
    });
})();
