
(function () {
    var reglas = {
        1: { max: 8, soloNumeros: true, ayuda: 'DNI: 8 dígitos' },                       // DNI
        2: { max: 12, soloNumeros: false, ayuda: 'Carné de extranjería: 9 a 12 caracteres' },
        3: { max: 12, soloNumeros: false, ayuda: 'Pasaporte: 6 a 12 caracteres' },
        4: { max: 10, soloNumeros: true, ayuda: 'Certificado de nacido vivo: 10 dígitos' }
    };

    var tipo = document.getElementById('tipoDocumento');
    var numero = document.getElementById('numeroDocumento');
    if (!tipo || !numero) return;

    var ayuda = document.createElement('div');
    ayuda.className = 'form-text';
    numero.parentNode.appendChild(ayuda);

    function limpiar() {
        var regla = reglas[tipo.value] || reglas[1];
        var valor = numero.value.toUpperCase();
        valor = regla.soloNumeros ? valor.replace(/[^0-9]/g, '') : valor.replace(/[^A-Z0-9]/g, '');
        numero.value = valor.slice(0, regla.max);
        numero.maxLength = regla.max;
        numero.inputMode = regla.soloNumeros ? 'numeric' : 'text';
        ayuda.textContent = regla.ayuda;
    }

    tipo.addEventListener('change', function () { numero.value = ''; limpiar(); });
    numero.addEventListener('input', limpiar);
    limpiar();
})();