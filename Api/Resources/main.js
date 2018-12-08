import "materialize-css";

require("./main.scss");

require("expose-loader?Ingredient!../Views/Ingredient/Ingredient");

document.addEventListener('DOMContentLoaded', function() {
    var sidenav = document.querySelector('.sidenav');
    M.Sidenav.init(sidenav);
});