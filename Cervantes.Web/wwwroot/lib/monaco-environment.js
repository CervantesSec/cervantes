// Monaco starts its web workers from blob: URLs by default, which the Content Security Policy refuses (script-src
// 'self', no worker-src). Loading the worker script from this origin instead keeps the CSP as it is.
self.MonacoEnvironment = {
    getWorkerUrl: function () {
        return '/_content/BlazorMonaco/lib/monaco-editor/min/vs/base/worker/workerMain.js';
    }
};
