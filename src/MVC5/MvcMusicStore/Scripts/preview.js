function getQueryParam(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}

// Save preview arguments
if (getQueryParam('preview') === '1') {
    document.cookie = "preview=1; path=/";
} else if (getQueryParam('preview') === '0') {
    document.cookie = "preview=0; path=/";
}