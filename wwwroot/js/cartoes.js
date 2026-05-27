const API = '/api/cartoes';
const token = localStorage.getItem('token') || '';

async function request(url, options = {}) {
    const response = await fetch(url, options);
    const text = await response.text();
    const data = text ? JSON.parse(text) : null;
    if (!response.ok) throw new Error(data?.erro || data?.title || 'Erro na solicitação.');
    return data;
}

async function solicitarCartao(e) {
    e?.preventDefault();
    try {
        await request(`${API}/solicitar`, {
            method: 'POST',
            headers: { Authorization: `Bearer ${token}` }
        });
        await carregarCartoes();
    } catch (error) {
        const msg = document.getElementById('mensagem');
        if (msg) msg.innerText = error.message;
    }
}

async function carregarCartoes() {
    try {
        const cartoes = await request(API, {
            headers: { Authorization: `Bearer ${token}` }
        });

        const lista = document.getElementById('listaCartoes');
        if (!lista) return;

        if (!Array.isArray(cartoes) || !cartoes.length) {
            lista.innerHTML = '<p>Nenhum cartão solicitado.</p>';
            return;
        }

        lista.innerHTML = cartoes.map(cartao => `
            <div class="card-cartao">
                <h3>${cartao.nomeTitular}</h3>
                <p>${cartao.numero}</p>
                <p>Validade: ${cartao.validade}</p>
                <p>CVV: ${cartao.cvv}</p>
                <p>Limite: R$ ${Number(cartao.limite || 0).toFixed(2)}</p>
            </div>
        `).join('');
    } catch (error) {
        console.error(error);
    }
}

document.getElementById('formCartao')?.addEventListener('submit', solicitarCartao);
carregarCartoes();
