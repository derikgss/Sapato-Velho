function atualizarIndicadorCarrinho() {
    const carrinho = JSON.parse(localStorage.getItem("carrinho")) || [];
    const quantidade = carrinho.length;
    const contador = document.getElementById("contadorCarrinho");
    const imagem = document.getElementById("imagemCarrinho");

    if (!contador || !imagem) return;

    contador.textContent = quantidade;

    if (quantidade > 0) {
        contador.classList.add("ativo");
        imagem.src = imagem.dataset.cheio;
        imagem.alt = "Carrinho com produtos";
    } else {
        contador.classList.remove("ativo");
        imagem.src = imagem.dataset.vazio;
        imagem.alt = "Carrinho vazio";
    }
}

document.addEventListener("DOMContentLoaded", atualizarIndicadorCarrinho);

window.addEventListener("storage", function(event) {
    if (event.key === "carrinho") {
        atualizarIndicadorCarrinho();
    }
});

const botaoFinalizar = document.getElementById("btn-finalizar");

function atualizarCheckout() {

    botaoFinalizar.disabled = carrinho.length === 0;

};

function irParaCheckout() {

    const itens = JSON.parse(localStorage.getItem("carrinho")) || [];

    if (itens.length === 0) {
        alert("Seu carrinho está vazio. Adicione um produto antes de finalizar o pedido.");
        return;
    }

    if (typeof estaLogado === "function" && !estaLogado()) {
        alert("Entre na sua conta para finalizar o pedido.");
        sessionStorage.setItem("voltarPara", "checkout.html");
        window.location.href = "login.html";
        return;
    }

    window.location.href = "checkout.html";
}
