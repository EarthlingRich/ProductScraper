export function initUpdate(keywords, allergyKeywords) {
    Utils.initSelect();

    var keywordsChipsElement = document.querySelector('#keywords-chips');
    var options = {
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

    M.Chips.init(keywordsChipsElement, options);

    var allergykeywordsChipsElement = document.querySelector('#allergykeywords-chips');
    var optionsAllergy = {
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

    M.Chips.init(allergykeywordsChipsElement, optionsAllergy);
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