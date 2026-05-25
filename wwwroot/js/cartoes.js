const API = 'http://localhost:5000/api/cartoes';

const form = document.getElementById('formCartao');
const numeroInput = document.getElementById('numero');
const validadeInput = document.getElementById('validade');

numeroInput.addEventListener('input', (e) => {
    let valor = e.target.value.replace(/\D/g, '');

    valor = valor.replace(/(.{4})/g, '$1 ').trim();

    e.target.value = valor;
});

validadeInput.addEventListener('input', (e) => {
    let valor = e.target.value.replace(/\D/g, '');

    if(valor.length > 2){
        valor = valor.slice(0,2) + '/' + valor.slice(2,4);
    }

    e.target.value = valor;
});

form.addEventListener('submit', async (e) => {

    e.preventDefault();

    const token = localStorage.getItem('token');

    const dados = {
        nomeTitular: document.getElementById('nomeTitular').value,
        numero: numeroInput.value.replace(/\s/g, ''),
        validade: validadeInput.value,
        cvv: document.getElementById('cvv').value,
        limite: parseFloat(document.getElementById('limite').value)
    };

    try{

        const response = await fetch(API, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(dados)
        });

        const resultado = await response.json();

        if(response.ok){

            document.getElementById('mensagem').innerText =
            'Cartão cadastrado com sucesso';

            form.reset();

            carregarCartoes();

        }else{

            document.getElementById('mensagem').innerText =
            resultado.erro || 'Erro ao cadastrar';

        }

    }catch(error){

        console.error(error);

        document.getElementById('mensagem').innerText =
        'Erro ao conectar com API';

    }
});

async function carregarCartoes(){

    const token = localStorage.getItem('token');

    try{

        const response = await fetch(API, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        const cartoes = await response.json();

        const lista = document.getElementById('listaCartoes');

        lista.innerHTML = '';

        if(!Array.isArray(cartoes)){
            return;
        }

        cartoes.forEach(cartao => {

            const numeroMascarado =
                cartao.numero.replace(/(\d{4})(?=\d)/g, '$1 ');

            lista.innerHTML += `
                <div class="card-cartao">
                    <h3>${cartao.nomeTitular}</h3>
                    <p>${numeroMascarado}</p>
                    <p>Validade: ${cartao.validade}</p>
                    <p>Limite: R$ ${cartao.limite}</p>
                </div>
            `;
        });

    }catch(error){
        console.error(error);
    }
}

carregarCartoes();
