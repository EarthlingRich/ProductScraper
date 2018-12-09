export function getBaseUrl() {
    return window.location.protocol + '//' + window.location.host;
}

export function initDatatables() {
    $.extend($.fn.dataTable.defaults, {
        processing: true,
        serverSide: true,
        pageLength: 25,
        dom: 'tp',
        language: {
            'sProcessing': 'Bezig...',
            'sLengthMenu': '_MENU_ resultaten weergeven',
            'sZeroRecords': 'Geen resultaten gevonden',
            'sInfo': '_START_ tot _END_ van _TOTAL_ resultaten',
            'sInfoEmpty': 'Geen resultaten gevonden',
            'sInfoFiltered': ' (gefilterd uit _MAX_ resultaten)',
            'sInfoPostFix': '',
            'sSearch': 'Zoeken:',
            'sEmptyTable': 'Geen resultaten gevonden',
            'sInfoThousands': '.',
            'sLoadingRecords': 'Laden...',
            'oPaginate': {
                'sFirst': '<i class="material-icons">first_page</i>',
                'sLast':'<i class="material-icons">last_page</i>',
                'sPrevious': '<i class="material-icons">navigate_before</i>',
                'sNext': '<i class="material-icons">navigate_next</i>'
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
}