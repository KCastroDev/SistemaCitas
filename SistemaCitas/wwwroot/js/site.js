// ============================================================================
// Reglas de escritura de los formularios (validacion en el navegador).
// Impiden ESCRIBIR o PEGAR caracteres no permitidos. El servidor vuelve a validar todo
// (los ViewModels tienen las mismas reglas), asi que esto solo mejora la experiencia.
//
// Reglas:
//   - DNI ............. solo numeros, exactamente 8
//   - Telefono ........ solo numeros, 9 digitos, siempre empieza con 9
//   - CMP ............. solo numeros, 6 digitos maximo
//   - Nombres ......... solo letras y espacios, maximo 30
//   - Apellidos ....... solo letras y espacios, maximo 20
//   - Especialidad .... solo letras y espacios, maximo 80
//   - Correo .......... sin espacios, maximo 100
//   - Cupos por hora .. solo numeros, maximo 2 digitos
//   - Buscadores ...... maximo 50 caracteres
// ============================================================================
(function () {
    'use strict';

    var LETRAS = /[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ ]/g;

    // Cada regla: como encontrar el campo, que limpiar y el largo maximo
    var reglas = [
        { sel: '#Dni',                 limpiar: soloDigitos,           max: 8,   tipoTel: false },
        { sel: '#Telefono',            limpiar: telefono,              max: 9   },
        { sel: '#Cmp',                 limpiar: soloDigitos,           max: 6   },
        { sel: '#Nombres',             limpiar: soloLetras,            max: 30  },
        { sel: '#ApellidoPaterno',     limpiar: soloLetras,            max: 20  },
        { sel: '#ApellidoMaterno',     limpiar: soloLetras,            max: 20  },
        { sel: '#Nuevo_CuposPorHora',  limpiar: soloDigitos,           max: 2   },
        { sel: 'input[name="buscar"]', limpiar: function (v) { return v; }, max: 50 },
        { sel: 'input[type="email"], #Correo', limpiar: sinEspacios,   max: 100 }
    ];

    // En la pantalla de Especialidades el campo "Nombre" solo admite letras
    if (location.pathname.toLowerCase().indexOf('/especialidades') === 0) {
        reglas.push({ sel: '#Nombre', limpiar: soloLetras, max: 80 });
    }

    function soloDigitos(v) { return v.replace(/\D/g, ''); }

    function soloLetras(v) {
        return v.replace(LETRAS, '')      // quita numeros y signos
                .replace(/ {2,}/g, ' ')   // un solo espacio entre palabras
                .replace(/^ /, '');       // sin espacio al inicio
    }

    function sinEspacios(v) { return v.replace(/\s/g, ''); }

    // Telefono: solo numeros y el primer digito debe ser 9
    function telefono(v) { return v.replace(/\D/g, '').replace(/^[^9]+/, ''); }

    function aplicar(el, regla) {
        if (el.dataset.reglaAplicada) return;
        el.dataset.reglaAplicada = '1';
        el.setAttribute('maxlength', regla.max);
        if (regla.limpiar === soloDigitos || regla.limpiar === telefono) el.setAttribute('inputmode', 'numeric');
        el.setAttribute('autocomplete', el.getAttribute('autocomplete') || 'off');

        el.addEventListener('input', function () {
            var limpio = regla.limpiar(el.value).substring(0, regla.max);
            if (limpio !== el.value) el.value = limpio;
        });

        // Al salir del campo se quitan los espacios sobrantes del final
        el.addEventListener('blur', function () {
            if (regla.limpiar === soloLetras) el.value = el.value.replace(/ +$/, '');
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        reglas.forEach(function (r) {
            document.querySelectorAll(r.sel).forEach(function (el) { aplicar(el, r); });
        });

        // Fecha de nacimiento: no se puede elegir una fecha futura ni de hace mas de 120 anios
        var nac = document.getElementById('FechaNacimiento');
        if (nac) {
            var hoy = new Date();
            var iso = function (d) { return d.toISOString().substring(0, 10); };
            nac.setAttribute('max', iso(hoy));
            nac.setAttribute('min', (hoy.getFullYear() - 120) + '-01-01');
        }

        // Campos numericos (type="number"): no dejar escribir e, +, - ni punto
        document.querySelectorAll('input[type="number"]').forEach(function (el) {
            el.addEventListener('keydown', function (e) {
                if (['e', 'E', '+', '-', '.', ','].indexOf(e.key) !== -1) e.preventDefault();
            });
        });
    });
})();
