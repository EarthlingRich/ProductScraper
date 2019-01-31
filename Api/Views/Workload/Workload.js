export function initIndex() {
    Utils.initDatatables();

    $('#workload-table').DataTable({
        ajax: Utils.getBaseUrl() + '/Workload/ProductList',
        columns:
            [
                { data: 'productName' },
                { data: 'message' },
                { data: 'createdOn' } 
            ],
        columnDefs: [
            {
                targets: 0,
                render(data, type, row) {
                    var url = Utils.getBaseUrl() + '/Product/Update/' + row['productId'];
                    return `<a href=\"${url}\"})">${data}</a>`;
                }
            }
        ]
    });
}