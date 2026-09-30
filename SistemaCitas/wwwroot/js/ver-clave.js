// Boton "ojito" para mostrar u ocultar una contrasena.
// Uso en la vista:
//   <button type="button" data-ver-clave="#Contrasena"><i class="bi bi-eye"></i></button>
(function () {
    document.querySelectorAll('[data-ver-clave]').forEach(function (boton) {
        boton.addEventListener('click', function () {
            var campo = document.querySelector(boton.dataset.verClave);
            if (!campo) return;

            var oculto = campo.type === 'password';
            campo.type = oculto ? 'text' : 'password';

            var icono = boton.querySelector('i');
            if (icono) icono.className = oculto ? 'bi bi-eye-slash' : 'bi bi-eye';

            campo.focus();
        });
    });
})();