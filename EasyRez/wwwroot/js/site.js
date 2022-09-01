// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


var selectTipoPersona = document.getElementById("selAETipoPersona");
var divMensajeSelect = document.getElementById("divMessageSelect");
var txtRFC = document.getElementById("txtAERFC");
var btnSubmit = document.getElementById("btnSaveCapture");
var divPersonaFisica = document.querySelectorAll(".divPersonaFisica");
var divPersonaMoral = document.querySelectorAll(".divPersonaMoral");
var frmCaptura = document.getElementById("frmCaptura");

function showDivName() {  
    var selValue = selectTipoPersona.options[selectTipoPersona.selectedIndex].value;
    console.log("El valor del select al Cambiar es: " + selValue);

    if (selValue == 119) {
        for (var i = 0; i < divPersonaFisica.length; i++) {
            divPersonaFisica[i].classList.remove("d-none");
        }
        for (var i = 0; i < divPersonaMoral.length; i++) {
            divPersonaMoral[i].classList.add("d-none");
        }
        divMensajeSelect.classList.add("d-none");
    } else if (selValue == 120) {
        for (var i = 0; i < divPersonaFisica.length; i++) {
            divPersonaFisica[i].classList.add("d-none");
        }
        for (var i = 0; i < divPersonaMoral.length; i++) {
            divPersonaMoral[i].classList.remove("d-none");
        }
        divMensajeSelect.classList.add("d-none");
    } /*else {
        for (var i = 0; i < divPersonaFisica.length; i++) {
            divPersonaFisica[i].classList.add("d-none");
        }
        for (var i = 0; i < divPersonaMoral.length; i++) {
            divPersonaMoral[i].classList.add("d-none");
        }
        divMensajeSelect.classList.remove("d-none");
    }*/
}