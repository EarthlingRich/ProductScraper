export function initIndex() {
    Utils.initDatatables();

    $('#product-table').DataTable({
        ajax: Utils.getBaseUrl() + '/Product/ProductList',
        columns:
            [
                { data: 'name' },
                { data: 'veganType' },
                { data: 'productCategories' }
            ],
        columnDefs: [
            {
                targets: 0,
                render(data, type, row) {
                    var url = Utils.getBaseUrl() + "/Product/Update/" + row["id"];
                    return `<a href=\"${url}\"})">${data}</a>`;
                }
            }
        ]
    });
}

export function initUpdate() {
    Utils.initSelect();
}

export function initProductActivityList() {
    Utils.initDatatables();

    $('#productactivity-table').DataTable({
        ajax: Utils.getBaseUrl() + '/Product/ProductActivities',
        columns:
            [
                { data: 'productName' },
                { data: 'type' },
                { data: 'detail' }
            ],
        columnDefs: [
            {
                targets: 0,
                render(data, type, row) {
                    var url = Utils.getBaseUrl() + "/Product/Update/" + row["productId"];
                    return `<a href=\"${url}\"})">${data}</a>`;
                }
            }
        ]
    });
}
