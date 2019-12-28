import Tagify from '@yaireo/tagify'

export function initIndex() {
    Utils.initDatatables();

    $('#ingredient-table').DataTable({
        ajax: Utils.getBaseUrl() + '/Ingredient/IngredientList',
        columns:
            [
                { data: 'name' },
                { data: 'veganType' },
            ],
        columnDefs: [
            {
                targets: 0,
                render(data, type, row) {
                    var url = Utils.getBaseUrl() + "/Ingredient/Update/" + row["id"];
                    return `<a href=\"${url}\"})">${data}</a>`;
                }
            }
        ]
    });
}

export function initUpdate() {
    var inputKeywords = document.querySelector('#Keywords');
    new Tagify(inputKeywords);

    var inputIgnoreKeywords = document.querySelector('#IgnoreKeywords');
    new Tagify(inputIgnoreKeywords);

    var inputAllergyKeywords = document.querySelector('#AllergyKeywords');
    new Tagify(inputAllergyKeywords);
}