export function initLayout() {
    initSidenav();
    Utils.fixSelectBoxes();
}

function initSidenav() {
    var sidenav = document.querySelector('.sidenav');
    M.Sidenav.init(sidenav);

    var collapsibleElements = document.querySelectorAll('.collapsible');
    M.Collapsible.init(collapsibleElements);
}
