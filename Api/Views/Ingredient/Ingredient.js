var keywordsChips = null;
var allergykeywordsChips = null;

export function initUpdate(keywords, allergyKeywords) {
    Utils.initSelect();

    var keywordsChipsElement = document.querySelector('#keywords-chips');

    var options = {
        data: keywords,
        onChipAdd: function() {
            UpdateKeywordsString();
        },
        onChipDelete: function() {
            UpdateKeywordsString();
        }
    }

    keywordsChips = M.Chips.init(keywordsChipsElement, options);

    var allergykeywordsChipsElement = document.querySelector('#allergykeywords-chips');

    var optionsAllergy = {
        data: allergyKeywords,
        onChipAdd: function() {
            UpdateAllergyKeywordsString();
        },
        onChipDelete: function() {
            UpdateAllergyKeywordsString();
        }
    }

    allergykeywordsChips = M.Chips.init(allergykeywordsChipsElement, optionsAllergy);
}

function UpdateKeywordsString() {
    var data = '';

    keywordsChips.chipsData.forEach(function(item) {
        data += item.tag + ';';
    });
    data = data.substring(0, data.length - 1);
    var keyWordsStringElement = document.getElementById('Request_KeywordsString');
    keyWordsStringElement.value = data;
}

function UpdateAllergyKeywordsString() {
    var data = '';

    allergykeywordsChips.chipsData.forEach(function(item) {
        data += item.tag + ';';
    });
    data = data.substring(0, data.length - 1);
    var keyWordsStringElement = document.getElementById('Request_AllergyKeywordsString');
    keyWordsStringElement.value = data;
}