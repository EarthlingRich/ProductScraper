require("expose-loader?jQuery!expose-loader?$!jquery");
import "datatables.net";

require("./main.scss");

require("expose-loader?Utils!./Utils");
require("expose-loader?Ingredient!../Views/Ingredient/Ingredient");
require("expose-loader?Layout!../Views/Shared/Layout");
require("expose-loader?Product!../Views/Product/Product");
require("expose-loader?Workload!../Views/Workload/Workload");


$(function() {
    Layout.initLayout();
});