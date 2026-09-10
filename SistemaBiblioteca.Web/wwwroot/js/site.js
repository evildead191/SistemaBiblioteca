window.BibliotecaUI = {
    async mostrarExito(mensaje, titulo = "Operación realizada") {
        if (typeof Swal === "undefined") {
            alert(mensaje);
            return;
        }

        await Swal.fire({
            icon: "success",
            title: titulo,
            text: mensaje,
            confirmButtonText: "Aceptar",
            confirmButtonColor: "#075C32",
            allowOutsideClick: false
        });
    },

    async mostrarError(
        mensaje = "Ocurrió un error al procesar la solicitud.",
        titulo = "No fue posible completar la operación"
    ) {
        if (typeof Swal === "undefined") {
            alert(mensaje);
            return;
        }

        await Swal.fire({
            icon: "error",
            title: titulo,
            text: mensaje,
            confirmButtonText: "Aceptar",
            confirmButtonColor: "#866144"
        });
    },

    async confirmar({
        titulo,
        mensaje,
        textoConfirmar = "Confirmar",
        textoCancelar = "Cancelar",
        icono = "question"
    }) {
        if (typeof Swal === "undefined") {
            return confirm(mensaje);
        }

        const resultado = await Swal.fire({
            icon: icono,
            title: titulo,
            text: mensaje,
            showCancelButton: true,
            confirmButtonText: textoConfirmar,
            cancelButtonText: textoCancelar,
            confirmButtonColor: "#075C32",
            cancelButtonColor: "#6c757d",
            reverseButtons: true,
            focusCancel: true,
            allowOutsideClick: false
        });

        return resultado.isConfirmed;
    },

    bloquearBoton(boton, texto = "Procesando...") {
        if (!boton) {
            return;
        }

        boton.dataset.textoOriginal = boton.innerHTML;
        boton.disabled = true;

        boton.innerHTML = `
            <span class="spinner-border spinner-border-sm me-2"
                  role="status"
                  aria-hidden="true">
            </span>
            ${texto}
        `;
    },

    restaurarBoton(boton) {
        if (!boton) {
            return;
        }

        boton.disabled = false;

        if (boton.dataset.textoOriginal) {
            boton.innerHTML = boton.dataset.textoOriginal;
            delete boton.dataset.textoOriginal;
        }
    }
};


/*
 * Control de inactividad de la sesión.
 */
(() => {
    const formularioLogout =
        document.getElementById("formLogoutInactividad");

    if (!formularioLogout) {
        return;
    }

    // TEMPORAL PARA PRUEBAS: 15 minutos.
const tiempoMaximoInactividad = 15 * 60 * 1000;
    let temporizadorInactividad;
    let cerrandoSesion = false;

    function cerrarSesionPorInactividad() {
        if (cerrandoSesion) {
            return;
        }

        cerrandoSesion = true;

        const campoMotivo =
            formularioLogout.querySelector(
                'input[name="motivo"]');

        if (campoMotivo) {
            campoMotivo.value = "inactividad";
        }

        formularioLogout.submit();
    }

    function reiniciarTemporizador() {
        if (cerrandoSesion) {
            return;
        }

        clearTimeout(temporizadorInactividad);

        temporizadorInactividad =
            setTimeout(
                cerrarSesionPorInactividad,
                tiempoMaximoInactividad);
    }

    const eventosActividad = [
        "mousedown",
        "keydown",
        "touchstart",
        "scroll"
    ];

    eventosActividad.forEach(evento => {
        document.addEventListener(
            evento,
            reiniciarTemporizador,
            { passive: true });
    });

    reiniciarTemporizador();
})();