export function initLayout() {
    initSidenav();
}

function initSidenav() {
    var sidenav = document.querySelector('.sidenav');
    M.Sidenav.init(sidenav);
}
