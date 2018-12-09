var chips = null;

export function initUpdate(data) {
    var chipsElement = document.querySelector('.chips');

    var options = {
        data: data,
        onChipAdd: function() {
            UpdateKeyWordsString();
        },
        onChipDelete: function() {
            UpdateKeyWordsString();
        }
    }

    chips = M.Chips.init(chipsElement, options);
}

function UpdateKeyWordsString() {
    var data = '';

    chips.chipsData.forEach(function(item) {
        data += item.tag + ';';
    });
    data = data.substring(0, data.length - 1);
    var keyWordsStringElement = document.getElementById('KeyWordsString');
    keyWordsStringElement.value = data;
}