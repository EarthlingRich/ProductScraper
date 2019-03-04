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

export function initUpdate(allergyKeywords, ignoreKeywords, keywords) {
    Utils.initSelect();

    var allergyKeywordsChipsElement = document.querySelector('#allergyKeywords-chips');
    var optionsAllergyKeywords = {
        data: allergyKeywords,
        placeholder: 'Allergiën',
        secondaryPlaceholder: 'allergie',
        onChipAdd: function() {
            UpdateKeywordsString('Request_AllergyKeywordsString', this);
        },
        onChipDelete: function() {
            UpdateKeywordsString('Request_AllergyKeywordsString', this);
        }
    }

    M.Chips.init(allergyKeywordsChipsElement, optionsAllergyKeywords);

    var ignoreKeywordsChipsElement = document.querySelector('#ignoreKeywords-chips');
    var optionsIgnoreKeywords = {
        data: ignoreKeywords,
        placeholder: 'Negeren',
        secondaryPlaceholder: 'negeren',
        onChipAdd: function() {
            UpdateKeywordsString('Request_IgnoreKeywordsString', this);
        },
        onChipDelete: function() {
            UpdateKeywordsString('Request_IgnoreKeywordsString', this);
        }
    }

    M.Chips.init(ignoreKeywordsChipsElement, optionsIgnoreKeywords);

    var keywordsChipsElement = document.querySelector('#keywords-chips');
    var optionsKeywords = {
        data: keywords,
        placeholder: 'Ingrediënten',
        secondaryPlaceholder: 'ingrediënt',
        onChipAdd: function() {
            UpdateKeywordsString('Request_KeywordsString', this);
        },
        onChipDelete: function() {
            UpdateKeywordsString('Request_KeywordsString', this);
        }
    }

    M.Chips.init(keywordsChipsElement, optionsKeywords);
}

function UpdateKeywordsString(id, chips) {
    var data = '';

    chips.chipsData.forEach(function(item) {
        data += item.tag + ';';
    });

    //Remove last semicolon
    data = data.substring(0, data.length - 1);

    var keyWordsStringElement = document.getElementById(id);
    keyWordsStringElement.value = data;
}