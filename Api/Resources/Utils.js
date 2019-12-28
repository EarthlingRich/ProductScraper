export function getBaseUrl() {
    return window.location.protocol + '//' + window.location.host;
}

export function initDatatables() {
    $.extend($.fn.dataTable.defaults, {
        searching: true,
        processing: true,
        serverSide: true,
        pageLength: 25,
        dom: '<"row"<"col-6"f>>tp',
        language: {
            'sProcessing': 'Bezig...',
            'sLengthMenu': '_MENU_ resultaten weergeven',
            'sZeroRecords': 'Geen resultaten gevonden',
            'sInfo': '_START_ tot _END_ van _TOTAL_ resultaten',
            'sInfoEmpty': 'Geen resultaten gevonden',
            'sInfoFiltered': ' (gefilterd uit _MAX_ resultaten)',
            'sInfoPostFix': '',
            'sSearch': '',
            'sEmptyTable': 'Geen resultaten gevonden',
            'sInfoThousands': '.',
            'sLoadingRecords': 'Laden...',
            'oPaginate': {
                'sFirst': '<i class="material-icons" aria-hidden="true">first_page</i>',
                'sLast':'<i class="material-icons" aria-hidden="true">last_page</i>',
                'sPrevious': '<i class="material-icons" aria-hidden="true">navigate_before</i>',
                'sNext': '<i class="material-icons" aria-hidden="true">navigate_next</i>'
            },
            'oAria': {
                'sSortAscending': ': activeer om kolom oplopend te sorteren',
                'sSortDescending': ': activeer om kolom aflopend te sorteren',
                'oPaginate': {
                    'sFirst': 'Eerste',
                    'sLast': 'Laatste',
                    'sNext': 'Volgende',
                    'sPrevious': 'Vorige'
                }
            }
        }
    });

    $.extend($.fn.dataTable.ext.classes, {
        sFilterInput: "form-control datatables-search-input",
        sPageButton: "paginate_button page-item"
    });

    $(document).on( 'preInit.dt', function() {
        var filter = $(".dataTables_filter");
        var label = filter.find("label");
        var input = filter.find("input");
        label.before(input);
        label.remove();
    });

    $.fn.dataTable.defaults.renderer = 'bootstrap';
    $.fn.dataTable.ext.renderer.pageButton.bootstrap = function (settings, host, idx, buttons, page, pages) {
        var api = new $.fn.dataTable.Api(settings);
        var classes = settings.oClasses;
        var lang = settings.oLanguage.oPaginate;
        var btnDisplay, btnClass;

        var attach = function (container, buttons) {
            var i, ien, node, button;
            var clickHandler = function (e) {
                e.preventDefault();
                if (e.data.action !== 'ellipsis') {
                    api.page(e.data.action).draw(false);
                }
            };

            for (i = 0, ien = buttons.length; i < ien; i++) {
                button = buttons[i];

                if ($.isArray(button)) {
                    attach(container, button);
                }
                else {
                    btnDisplay = '';
                    btnClass = '';

                    switch (button) {
                        case 'ellipsis':
                            btnDisplay = '&hellip;';
                            btnClass = 'disabled';
                            break;

                        case 'first':
                            btnDisplay = lang.sFirst;
                            btnClass = button + (page > 0 ?
                                '' : ' disabled');
                            break;

                        case 'previous':
                            btnDisplay = lang.sPrevious;
                            btnClass = button + (page > 0 ?
                                '' : ' disabled');
                            break;

                        case 'next':
                            btnDisplay = lang.sNext;
                            btnClass = button + (page < pages - 1 ?
                                '' : ' disabled');
                            break;

                        case 'last':
                            btnDisplay = lang.sLast;
                            btnClass = button + (page < pages - 1 ?
                                '' : ' disabled');
                            break;

                        default:
                            btnDisplay = button + 1;
                            btnClass = page === button ?
                                'active' : '';
                            break;
                    }

                    if (btnDisplay) {
                        node = $('<li>', {
                            'class': classes.sPageButton + ' ' + btnClass,
                            'aria-controls': settings.sTableId,
                            'tabindex': settings.iTabIndex,
                            'id': idx === 0 && typeof button === 'string' ?
                                settings.sTableId + '_' + button :
                                null
                        })
                            .append($('<a>', {
                                'href': '#',
                                'class': 'page-link'
                            })
                                .html(btnDisplay)
                            )
                            .appendTo(container);

                        settings.oApi._fnBindAction(
                            node, { action: button }, clickHandler
                        );
                    }
                }
            }
        };

        attach(
            $(host).empty().html('<ul class="pagination"/>').children('ul'),
            buttons
        );
    }
}

export function fixSelectBoxes() {
    var checkboxElements = document.querySelectorAll('input[type=checkbox]');
    checkboxElements.forEach(function(checkBoxElement) {
        var nextElement = checkBoxElement.nextElementSibling;
        if (nextElement.tagName.toLowerCase() === 'input' && nextElement.getAttribute('type') === 'hidden') {
            checkBoxElement.prepend(nextElement);
        }
    });
}

export function getStoreIcon(storeType) {
    var icon = "";
    
    if (storeType === 1) {
        icon = "<div class='store-icon store-icon--ah'>AH</div>"
    }
    else if (storeType === 2) {
        icon = "<div class='store-icon store-icon store-icon--jumbo'>J</div>"
    }

    return icon;
}