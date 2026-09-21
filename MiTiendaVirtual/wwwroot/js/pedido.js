// Formulario maestro-detalle de Pedidos: filas dinámicas y total en vivo.
(function () {
    const dataEl = document.getElementById('pedido-data');
    const body = document.getElementById('detalleBody');
    if (!dataEl || !body) return;

    const data = JSON.parse(dataEl.textContent);
    const productos = data.productos || [];
    const totalEl = document.getElementById('totalPedido');
    const vacioEl = document.getElementById('detalleVacio');

    const esc = s => String(s).replace(/[&<>"']/g, c =>
        ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
    const fmt = n => n.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    const redondear = n => Math.round((n + Number.EPSILON) * 100) / 100;

    const opciones = '<option value="">-- Seleccione --</option>' + productos.map(p =>
        `<option value="${p.id}" data-precio="${p.precio}">${esc(p.nombre)}</option>`).join('');

    function agregarFila(d) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td><select class="form-select form-select-sm js-prod" data-campo="IdProducto" required>${opciones}</select></td>
            <td><input type="number" class="form-control form-control-sm js-cant" data-campo="Cantidad" min="1" step="1" value="1" required></td>
            <td><input type="number" class="form-control form-control-sm js-precio" data-campo="PrecioUnitario" min="0" step="0.01" value="0" required></td>
            <td class="text-end js-sub">0.00</td>
            <td class="text-end"><button type="button" class="btn btn-sm btn-outline-danger js-quitar" title="Quitar producto"><i class="bi bi-x-lg"></i></button></td>`;
        body.appendChild(tr);

        if (d) {
            tr.querySelector('.js-prod').value = d.idProducto || '';
            tr.querySelector('.js-cant').value = d.cantidad || 1;
            tr.querySelector('.js-precio').value = d.precioUnitario ?? 0;
        }
        actualizar();
    }

    // Renumera los nombres (Detalles[0].X, Detalles[1].X...) para que el model binder
    // reciba una lista continua, y recalcula subtotales y total.
    function actualizar() {
        let total = 0;
        body.querySelectorAll('tr').forEach((tr, i) => {
            tr.querySelectorAll('[data-campo]').forEach(el => {
                el.name = `Detalles[${i}].${el.dataset.campo}`;
            });
            const cant = parseFloat(tr.querySelector('.js-cant').value) || 0;
            const precio = parseFloat(tr.querySelector('.js-precio').value) || 0;
            const sub = redondear(cant * precio);
            tr.querySelector('.js-sub').textContent = fmt(sub);
            total += sub;
        });
        totalEl.textContent = fmt(redondear(total));
        vacioEl.hidden = body.children.length > 0;
    }

    body.addEventListener('change', e => {
        if (e.target.classList.contains('js-prod')) {
            const opt = e.target.selectedOptions[0];
            const precio = opt && opt.dataset.precio ? parseFloat(opt.dataset.precio) : 0;
            e.target.closest('tr').querySelector('.js-precio').value = precio.toFixed(2);
        }
        actualizar();
    });

    body.addEventListener('input', e => {
        if (e.target.matches('.js-cant, .js-precio')) actualizar();
    });

    body.addEventListener('click', e => {
        const btn = e.target.closest('.js-quitar');
        if (!btn) return;
        btn.closest('tr').remove();
        actualizar();
    });

    document.getElementById('btnAgregar').addEventListener('click', () => agregarFila(null));

    const iniciales = data.detalles || [];
    if (iniciales.length) iniciales.forEach(d => agregarFila(d));
    else agregarFila(null);
})();
