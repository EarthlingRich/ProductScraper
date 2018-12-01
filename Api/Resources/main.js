import "materialize-css";

require("./main.scss");

document.addEventListener('DOMContentLoaded', function() {
    var sidenav = document.querySelector('.sidenav');
    M.Sidenav.init(sidenav);
});