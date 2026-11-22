const barraPesquisa = document.getElementById("barraPesquisa");
const produtos = document.querySelectorAll(".card");

barraPesquisa.addEventListener("input", function () {

    const pesquisa = barraPesquisa.value.toLowerCase();

    produtos.forEach(function (card) {

        const nome = card.dataset.produto.toLowerCase();

        if (nome.includes(pesquisa)) {
            card.style.display = "";
        } else {
            card.style.display = "none";
        }

    });

});