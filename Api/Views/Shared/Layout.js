export function initLayout() {
    initSidenav();
    Utils.fixSelectBoxes();
}

function initSidenav() {
    var sidenav = document.querySelector('.sidenav');
    M.Sidenav.init(sidenav);
}
