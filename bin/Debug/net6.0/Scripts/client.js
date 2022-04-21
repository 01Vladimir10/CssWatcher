const endpoint = '[END_POINT]';
const connection = new signalR
    .HubConnectionBuilder()
    .withUrl(`${endpoint}/liveCss`)
    .configureLogging(signalR.LogLevel.Information)
    .build();

function removeCss(url) {
    const css = document.querySelector(`link[href="${url}"]`);
    if (css) {
        css.remove();
    }
}

const injectCss = (url) => {
    let head = document.getElementsByTagName('head')[0];
    const link = document.createElement('link');
    link.id   = new Date().getTime().toString();
    link.rel  = 'stylesheet';
    link.type = 'text/css';
    link.media = 'all';
    link.rel = 'stylesheet';
    link.href = url;
    head.appendChild(link);
}

async function start() {
    try {
        await connection.start();
        await connection.invoke('Subscribe');
        console.log('Live css started');
    } catch (err) {
        console.log('Error while starting connection, attempting reconnection...\n\terror:', err);
        setTimeout(start, 5000);
    }
}
connection.on('FileChanged', (url) => {
    console.log('File changed: ', url);
    removeCss(url);
    injectCss(url);
});
connection.on('FileRemoved', (url) => {
    console.log('File deleted: ', url);
    removeCss(url);
});
connection.on('FileAdded', (url) => {
    console.log('File added: ', url);
    injectCss(url);
});
connection.on('Initialized', (urls) => {
    console.log('Initialized: ', urls);
    urls.forEach(url => {
        injectCss(url);
    });
});
connection.onclose(async () => {
    await start();
});
window.addEventListener('load', start);