let recorder;
let audioCtx;

export function startRecording() {
    navigator.getUserMedia({ audio: true }, onSuccess, onError);
}

let onSuccess = function (stream) {
    let context;

    let mainSection = document.querySelector('.main-controls');
    const canvas = document.querySelector('.visualizer');
    canvas.width = mainSection.offsetWidth;

    const canvasCtx = canvas.getContext("2d");

    context = new AudioContext();
    let mediaStreamSource = context.createMediaStreamSource(stream);
    
    recorder = new Recorder(mediaStreamSource);
    recorder.record();

    visualize(stream, canvas, canvasCtx);
}

export function stopRecording (dotnethelper) {
    recorder.stop();    
    recorder.exportWAV(function (s) {        
        let blob = window.URL.createObjectURL(s);
        let base64 ="";
        blobToBase64(s).then(res => {
            base64 = res;
            dotnethelper.invokeMethodAsync("setAudio", blob, base64);
        });
    });
}

const blobToBase64 = blob => {
    const reader = new FileReader();
    reader.readAsDataURL(blob);
    return new Promise(resolve => {
      reader.onloadend = () => {
        resolve(reader.result);
      };
    });
  };

let onError = function (err) {
    console.log('The following error occurred: ' + err);
};

function visualize(stream, canvas, canvasCtx) {
    if (!audioCtx) {
        audioCtx = new AudioContext();
    }

    const source = audioCtx.createMediaStreamSource(stream);

    const analyser = audioCtx.createAnalyser();
    analyser.fftSize = 2048;
    const bufferLength = analyser.frequencyBinCount;
    const dataArray = new Uint8Array(bufferLength);

    source.connect(analyser);
    //analyser.connect(audioCtx.destination);

    draw()

    function draw() {
        const WIDTH = canvas.width
        const HEIGHT = canvas.height;

        requestAnimationFrame(draw);

        analyser.getByteTimeDomainData(dataArray);

        canvasCtx.fillStyle = 'rgb(200, 200, 200)';
        canvasCtx.fillRect(0, 0, WIDTH, HEIGHT);

        canvasCtx.lineWidth = 2;
        canvasCtx.strokeStyle = 'rgb(0, 0, 0)';

        canvasCtx.beginPath();

        let sliceWidth = WIDTH * 1.0 / bufferLength;
        let x = 0;


        for (let i = 0; i < bufferLength; i++) {

            let v = dataArray[i] / 128.0;
            let y = v * HEIGHT / 2;

            if (i === 0) {
                canvasCtx.moveTo(x, y);
            } else {
                canvasCtx.lineTo(x, y);
            }

            x += sliceWidth;
        }

        canvasCtx.lineTo(canvas.width, canvas.height / 2);
        canvasCtx.stroke();

    }
}
