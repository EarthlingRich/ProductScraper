export function initIndex() {
    Utils.initDatatables();

    $('#workload-table').DataTable({
        ajax: Utils.getBaseUrl() + '/Workload/ProductList',
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
                        return `<i class="material-icons" aria-hidden="true">done</i>`;
                    }
                    return null;
                }
            }
        ]
    });
}