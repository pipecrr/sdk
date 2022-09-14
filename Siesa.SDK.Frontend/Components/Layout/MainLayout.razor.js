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