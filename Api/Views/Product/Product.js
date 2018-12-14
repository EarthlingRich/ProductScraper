export function initIndex() {
    Utils.initDatatables();

    $('#product-table').DataTable({
        ajax: Utils.getBaseUrl() + '/Product/ProductList',
        columns:
            [
                { data: 'name' },
                { data: 'veganType' }
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

export function initWorkload() {
    Utils.initDatatables();

    $('#workload-table').DataTable({
        ajax: Utils.getBaseUrl() + '/Product/WorkloadList',
        columns:
            [
                { data: 'name' },
                { data: 'isNew' }
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
                    if(data) {
                        return `<i class="material-icons">done</i>`;
                    }
                    return null;
                }
            }
        ]
    });
}
