var keywordsChips = null;
var allergykeywordsChips = null;

export function initUpdate(keywords, allergyKeywords) {
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

    Utils.initSelect();
}

function UpdateKeywordsString() {
    var data = '';

    keywordsChips.chipsData.forEach(function(item) {
        data += item.tag + ';';
    });
    data = data.substring(0, data.length - 1);
    var keyWordsStringElement = document.getElementById('KeywordsString');
    keyWordsStringElement.value = data;
}

function UpdateAllergyKeywordsString() {
    var data = '';

    allergykeywordsChips.chipsData.forEach(function(item) {
        data += item.tag + ';';
    });
    data = data.substring(0, data.length - 1);
    var keyWordsStringElement = document.getElementById('AllergyKeywordsString');
    keyWordsStringElement.value = data;
}