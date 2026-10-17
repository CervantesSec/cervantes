// Called from VulnDialog after render (JS.InvokeVoidAsync("initializePrism")). A file, not an inline script, so the
// page works under script-src 'self'.
window.initializePrism = () => {
    if (typeof Prism !== 'undefined') {
        Prism.highlightAll();
        // highlightAll skips a pre.language-x without a <code> child.
        document.querySelectorAll('pre[class*="language-"], code[class*="language-"]')
            .forEach((block) => Prism.highlightElement(block));
    } else {
        console.error('Prism is not available');
    }
};
