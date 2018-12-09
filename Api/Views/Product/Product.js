export function initIndex() {
    Utils.initDatatables();
    $('#product-table').DataTable({
        ajax: 'Product/ProductList',
        columns: [
            { data: 'name' }
        ],
        columnDefs: [
            {
                targets: 0,
                render(data, type, row) {
                    var url = "Product/Update/" + row["id"];
                    return `<a href=\"${url}\"})">${data}</a>`;
                }
            }
        ]
    });
}
