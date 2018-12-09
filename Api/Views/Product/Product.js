export function initIndex() {
    Utils.initDatatables();
    $('#product-table').DataTable({
        ajax: 'Product/ProductList',
        columns:
            [
                { data: 'name' },
                { data: 'isVegan' }
            ],
        columnDefs: [
            {
                targets: 0,
                render(data, type, row) {
                    var url = "Product/Update/" + row["id"];
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
