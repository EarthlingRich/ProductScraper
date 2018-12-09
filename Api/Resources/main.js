import "datatables.net";
import "materialize-css";
require("expose-loader?$!jquery");

require("./main.scss");

require("expose-loader?Ingredient!../Views/Ingredient/Ingredient");
require("expose-loader?Product!../Views/Product/Product");
require("expose-loader?Utils!./Utils");

document.addEventListener('DOMContentLoaded', function() {
    var sidenav = document.querySelector('.sidenav');
    M.Sidenav.init(sidenav);
});
