$(function () {
    // Grillas con búsqueda, orden y paginación
    $('table.grid').DataTable({
        pageLength: 10,
        order: [],
        columnDefs: [{ targets: -1, orderable: false, searchable: false }],
        language: {
            search: 'Buscar:',
            lengthMenu: 'Mostrar _MENU_ registros',
            info: 'Mostrando _START_ a _END_ de _TOTAL_ registros',
            infoEmpty: 'Sin registros',
            infoFiltered: '(filtrado de _MAX_)',
            zeroRecords: 'No se encontraron resultados',
            emptyTable: 'No hay registros',
            paginate: { first: 'Primero', last: 'Último', next: 'Siguiente', previous: 'Anterior' }
        }
    });

    // Confirmación de eliminación (un solo modal para todas las grillas)
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-delete-url]');
        if (!btn) return;

        document.getElementById('deleteForm').action = btn.dataset.deleteUrl;
        document.getElementById('deleteNombre').textContent = btn.dataset.nombre || 'este registro';
        bootstrap.Modal.getOrCreateInstance(document.getElementById('deleteModal')).show();
    });
});
