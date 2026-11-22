// ===== Sapato Velho - ligação com a API (.NET) =====
// Se a API rodar em outra porta, troque só esta linha.
const API_URL = "http://localhost:5141/api";

// ---------- sessão (token JWT) ----------
function lerToken() {
    return localStorage.getItem("token");
}

function dadosDoToken(token) {
    try {
        const base64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
        return JSON.parse(atob(base64));
    } catch {
        return null;
    }
}

function limparSessao() {
    localStorage.removeItem("token");
    localStorage.removeItem("usuario");
    localStorage.removeItem("logado");
}

function estaLogado() {
    const token = lerToken();
    if (!token) return false;

    const dados = dadosDoToken(token);
    if (!dados || !dados.exp || dados.exp * 1000 <= Date.now()) {
        limparSessao(); // token vencido
        return false;
    }
    return true;
}

function lerUsuario() {
    try {
        return JSON.parse(localStorage.getItem("usuario"));
    } catch {
        return null;
    }
}

function salvarSessao(token, usuario) {
    localStorage.setItem("token", token);
    localStorage.setItem("usuario", JSON.stringify(usuario));
    localStorage.setItem("logado", "true");
}

function sair() {
    limparSessao();
    window.location.href = caminhoRaiz() + "main.html";
}

// Páginas dentro de /produtos/ precisam de "../" para chegar na raiz do site.
function caminhoRaiz() {
    return window.location.pathname.includes("/produtos/") ? "../" : "";
}

// ---------- chamadas à API ----------
async function api(caminho, opcoes = {}) {
    const headers = { ...(opcoes.headers || {}) };
    const temCorpo = opcoes.body !== undefined;

    if (temCorpo) headers["Content-Type"] = "application/json";

    const token = lerToken();
    if (token) headers["Authorization"] = "Bearer " + token;

    let resposta;
    try {
        resposta = await fetch(API_URL + caminho, {
            ...opcoes,
            headers,
            body: temCorpo ? JSON.stringify(opcoes.body) : undefined
        });
    } catch {
        throw new Error("Não foi possível conectar à API. Verifique se ela está em execução.");
    }

    let dados = null;
    const texto = await resposta.text();
    if (texto) {
        try { dados = JSON.parse(texto); } catch { dados = texto; }
    }

    if (resposta.status === 401 && token) limparSessao(); // token vencido ou inválido

    if (!resposta.ok) {
        let mensagem = "Erro " + resposta.status + " ao falar com a API.";
        if (dados && dados.mensagem) mensagem = dados.mensagem;
        else if (dados && dados.errors) mensagem = Object.values(dados.errors).flat().join(" ");
        else if (dados && dados.title) mensagem = dados.title;

        const erro = new Error(mensagem);
        erro.status = resposta.status;
        throw erro;
    }

    return dados;
}

// ---------- utilidades ----------
function formatarPreco(valor) {
    return "R$ " + Number(valor).toFixed(2).replace(".", ",");
}

function escaparHtml(texto) {
    return String(texto)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");
}

function normalizarTexto(texto) {
    return (texto || "")
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase()
        .replace(/\s+/g, " ")
        .trim();
}

// ---------- carrinho ----------
function lerCarrinho() {
    try {
        return JSON.parse(localStorage.getItem("carrinho")) || [];
    } catch {
        return [];
    }
}

function salvarCarrinho(carrinho) {
    localStorage.setItem("carrinho", JSON.stringify(carrinho));
}

// Procura o produto na API pelo nome (as páginas de produto só informam o nome).
async function buscarProdutoPorNome(nome) {
    const lista = await api("/produtos");
    return lista.find(p => normalizarTexto(p.nome) === normalizarTexto(nome));
}

async function adicionarCarrinho(nome, preco, imagem, produtoId) {
    if (!produtoId) {
        try {
            const produto = await buscarProdutoPorNome(nome);
            if (produto) {
                produtoId = produto.id;
                preco = produto.preco;
            }
        } catch {
            // API fora do ar: o produto entra no carrinho e o id é procurado no checkout.
        }
    }

    const carrinho = lerCarrinho();
    carrinho.push({
        produtoId: produtoId || null,
        nome: nome,
        preco: Number(preco),
        imagem: String(imagem).replace(/^(\.\.\/)+/, "") // sempre relativo à raiz do site
    });
    salvarCarrinho(carrinho);

    if (typeof atualizarIndicadorCarrinho === "function") atualizarIndicadorCarrinho();
    alert(nome + " adicionado ao carrinho!");
}

// ---------- menu: "Entrar" vira "Minha Conta" quando há sessão ----------
function atualizarMenuLogin() {
    if (!estaLogado()) return;

    document.querySelectorAll("a.login-btn").forEach(link => {
        const href = link.getAttribute("href") || "";
        link.textContent = "Minha Conta";
        link.setAttribute("href", href.replace(/(login|perfil)\.html/, "conta.html"));
    });
}

document.addEventListener("DOMContentLoaded", atualizarMenuLogin);
