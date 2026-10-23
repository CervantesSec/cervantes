// Monaco is large and only the report component dialogs use it, so its scripts load on first use instead of on every
// page. Both are same-origin, so the Content Security Policy (script-src 'self') allows them.
window.cervantesMonaco = (function () {
    var base = '_content/BlazorMonaco/lib/monaco-editor/min/vs/';
    var loading = null;

    function load(src) {
        return new Promise(function (resolve, reject) {
            var script = document.createElement('script');
            script.src = src;
            script.onload = function () { resolve(); };
            script.onerror = function () { reject(new Error('Could not load ' + src)); };
            document.body.appendChild(script);
        });
    }

    return {
        ensure: function () {
            if (window.monaco && window.monaco.editor) {
                return Promise.resolve();
            }
            if (!loading) {
                // editor.main.js needs the AMD loader, so they load in order.
                loading = load(base + 'loader.js')
                    .then(function () { return load(base + 'editor/editor.main.js'); })
                    .catch(function (error) { loading = null; throw error; });
            }
            return loading;
        }
    };
})();
