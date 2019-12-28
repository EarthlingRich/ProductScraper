export function initIndex() {
    Utils.initDatatables();

    $('#product-table').DataTable({
        ajax: Utils.getBaseUrl() + '/Product/ProductList',
        columns:
            [
                { data: 'name' },
                { data: 'storeType' },
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
            },
            {
                targets: 1,
                render(data, type, row) {
                    return Utils.getStoreIcon(data);
                }
            }
        ]
    });
}

export function initProductActivityList() {
    Utils.initDatatables();

    $('#productactivity-table').DataTable({
        ajax: Utils.getBaseUrl() + '/Product/ProductActivities',
        columns:
            [
                { data: 'productName' },
                { data: 'productStoreType' },
                { data: 'type' },
                { data: 'detail' },
                { data: 'createdOn' }
            ],
        columnDefs: [
            {
                targets: 0,
                render(data, type, row) {
                    var url = Utils.getBaseUrl() + "/Product/Update/" + row["productId"];
                    return `<a href=\"${url}\"})">${data}</a>`;
                }
            },
            {
                targets: 1,
                render(data, type, row) {
                    return Utils.getStoreIcon(data);
                }
            }
        ]
    });
}

export function deleteProductActivity(productActivityId) {
    $.ajax({
        type: "POST",
        url: Utils.getBaseUrl() + "/Product/DeleteProductActivity",
        data: {
            productActivityId: productActivityId
        },
        success: function () {
            $("#product-activity-row-" + productActivityId).remove();
        }
    });
}

export function deleteWorkloadItem(workloadItemId) {
    $.ajax({
        type: "POST",
        url: Utils.getBaseUrl() + "/Product/DeleteWorkloadItem",
        data: {
            workloadItemId: workloadItemId
        },
        success: function () {
            $("#workload-item-row-" + workloadItemId).remove();
        }
    });
}