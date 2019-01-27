import "datatables.net";
import "materialize-css";
require("expose-loader?$!jquery");

require("./main.scss");

require("expose-loader?Ingredient!../Views/Ingredient/Ingredient");
require("expose-loader?Layout!../Views/Shared/Layout");
require("expose-loader?Product!../Views/Product/Product");
require("expose-loader?ProductCategory!../Views/ProductCategory/ProductCategory");
require("expose-loader?Utils!./Utils");
require("expose-loader?Workload!../Views/Workload/Workload");

$(function() {
    Layout.initLayout();
});