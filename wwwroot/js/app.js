
const api = '/api';
const money = new Intl.NumberFormat('pt-BR',{style:'currency',currency:'BRL'});
const dateFmt = new Intl.DateTimeFormat('pt-BR',{dateStyle:'short',timeStyle:'short'});
const tipoConta = {1:'Corrente',2:'Poupança',3:'Empresarial',Corrente:'Corrente',Poupanca:'Poupança',Empresarial:'Empresarial'};
let token = localStorage.getItem('token') || '';
let usuarioAtual = JSON.parse(localStorage.getItem('usuarioAtual') || 'null');
let contasCache = [];
const $ = id => document.getElementById(id);
function headers(){return {'Content-Type':'application/json',Authorization:`Bearer ${token}`};}
async function request(url,options={}){const res=await fetch(url,options);const text=await res.text();let data=null;if(text){try{data=JSON.parse(text)}catch{data=text}}if(!res.ok)throw new Error(data?.erro||data?.title||data||'Não foi possível concluir a solicitação.');return data;}
function toast(title,message){const area=$('toastArea');if(!area)return alert(`${title}\n${message}`);const el=document.createElement('div');el.className='toast-card';el.innerHTML=`<strong>${title}</strong><p>${message}</p>`;area.appendChild(el);setTimeout(()=>el.remove(),4200)}
function salvarSessao(data){token=data.token;usuarioAtual={nome:data.nome,email:data.email,perfil:data.perfil};localStorage.setItem('token',token);localStorage.setItem('usuarioAtual',JSON.stringify(usuarioAtual));}
function sair(){localStorage.removeItem('token');localStorage.removeItem('usuarioAtual');token='';usuarioAtual=null;window.location.href='/login.html'}
function protegerPagina(){if(!token)window.location.href='/login.html'}
function atualizarSessaoHome(){const el=$('sessionStatus');if(!el)return;if(token&&usuarioAtual)el.innerHTML=`<i class="bi bi-shield-check"></i><span>${usuarioAtual.nome}</span>`;else el.innerHTML='<i class="bi bi-shield-lock"></i><span>Sessão não iniciada</span>';}
async function login(event){event?.preventDefault();try{const email=$('email').value.trim();const senha=$('senha').value;if(!email||!senha)throw new Error('Preencha e-mail e senha.');const data=await request(`${api}/auth/login`,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({email,senha})});salvarSessao(data);toast('Login realizado','Redirecionando para o dashboard.');setTimeout(()=>window.location.href='/operacoes.html',450)}catch(e){toast('Erro no login',e.message)}}
async function registrar(event){event?.preventDefault();try{const nome=$('nome').value.trim();const email=$('email').value.trim();const senha=$('senha').value;if(!nome||!email||!senha)throw new Error('Preencha nome, e-mail e senha.');const data=await request(`${api}/auth/registrar`,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({nome,email,senha})});salvarSessao(data);toast('Cadastro realizado','Sua sessão foi iniciada.');setTimeout(()=>window.location.href='/operacoes.html',450)}catch(e){toast('Erro no cadastro',e.message)}}
function converterMoedaParaNumero(valor){if(typeof valor!=='string')return Number(valor||0);return Number(valor.replace(/\./g,'').replace(',','.'))||0}
function aplicarMascaraDinheiroInput(event){let valor=event.target.value.replace(/\D/g,'');valor=(Number(valor)/100).toFixed(2).replace('.',',');valor=valor.replace(/(\d)(?=(\d{3})+(?!\d))/g,'$1.');event.target.value=valor}
function setSelectValue(el,id){if(!el)return;el.value=String(id)}
function renderContaOptions(){document.querySelectorAll('.conta-select').forEach(select=>{const atual=select.value;if(!contasCache.length){select.innerHTML='<option value="">Nenhuma conta encontrada</option>';return}select.innerHTML='<option value="">Selecione uma conta</option>'+contasCache.map(c=>`<option value="${c.id}">Conta #${c.id} • ${tipoConta[c.tipo]||c.tipo} • ${money.format(Number(c.saldo||0))}</option>`).join('');if(atual)setSelectValue(select,atual)});}
async function listarContas(){try{if(token)protegerPagina();contasCache=await request(`${api}/contas`,{headers:headers()})||[];const metricContas=$('metricContas'),metricSaldo=$('metricSaldo');if(metricContas)metricContas.textContent=contasCache.length;if(metricSaldo)metricSaldo.textContent=money.format(contasCache.reduce((acc,c)=>acc+Number(c.saldo||0),0));renderContaOptions();const lista=$('contas');if(!lista)return;if(!contasCache.length){lista.innerHTML='<div class="empty-state"><p>Nenhuma conta criada ainda. Vá em Contas para criar sua primeira conta.</p></div>';return}lista.innerHTML=contasCache.map(c=>`<article class="account-card"><div><h3>Conta #${c.id}</h3><div class="account-meta"><span class="tag"><i class="bi bi-wallet2"></i>${tipoConta[c.tipo]||c.tipo}</span><span class="tag"><i class="bi bi-person"></i>Usuário ${c.usuarioId||''}</span></div><div class="account-actions"><button class="icon-btn" title="Selecionar" onclick="selecionarConta(${c.id})"><i class="bi bi-check2-circle"></i></button><button class="icon-btn" title="Extrato" onclick="selecionarConta(${c.id}); window.location.href='/extrato.html?conta=${c.id}'"><i class="bi bi-receipt"></i></button><button class="icon-btn" title="Excluir" onclick="remover(${c.id})"><i class="bi bi-trash"></i></button></div></div><div class="balance">${money.format(Number(c.saldo||0))}</div></article>`).join('')}catch(e){toast('Erro ao listar contas',e.message)}}
function selecionarConta(id){document.querySelectorAll('.conta-select').forEach(s=>s.value=String(id));const input=$('contaId');if(input)input.value=String(id);toast('Conta selecionada',`Conta #${id} selecionada.`)}
async function criarConta(event){event?.preventDefault();try{const tipo=Number($('tipo').value);const saldoInicial=converterMoedaParaNumero($('saldoInicial').value);await request(`${api}/contas`,{method:'POST',headers:headers(),body:JSON.stringify({tipo,saldoInicial})});$('saldoInicial').value='0,00';await listarContas();toast('Conta criada','A nova conta já aparece na lista.')}catch(e){toast('Erro ao criar conta',e.message)}}
async function movimentar(tipoOp){try{const id=Number($('contaId').value);const valor=converterMoedaParaNumero($('valor').value);if(!id||valor<=0)throw new Error('Selecione uma conta e informe um valor maior que zero.');const data=await request(`${api}/contas/${id}/${tipoOp}`,{method:'POST',headers:headers(),body:JSON.stringify({valor})});$('valor').value='';await listarContas();selecionarConta(id);toast(tipoOp==='depositar'?'Depósito concluído':'Saque concluído',`Novo saldo: ${money.format(Number(data?.saldoAtual||0))}`)}catch(e){toast('Erro na operação',e.message)}}
const depositar=()=>movimentar('depositar'); const sacar=()=>movimentar('sacar');
async function carregarExtrato(showEmptyToast=true){try{const id=Number($('contaId')?.value);if(!id){if($('extrato'))$('extrato').innerHTML='<div class="empty-state"><p>Selecione uma conta para ver o extrato.</p></div>';if(showEmptyToast)toast('Conta não selecionada','Escolha uma conta no seletor.');return}const data=await request(`${api}/contas/${id}/extrato`,{headers:headers()})||[];if(!data.length){$('extrato').innerHTML='<div class="empty-state"><p>Esta conta ainda não possui transações.</p></div>';return}$('extrato').innerHTML=data.slice().reverse().map(t=>{const deposito=String(t.tipo).toLowerCase().includes('deposito')||t.tipo===1;const icon=deposito?'arrow-down-left':'arrow-up-right';const label=deposito?'Depósito':'Saque';const sinal=deposito?'+':'-';return `<div class="statement-item"><div class="statement-icon"><i class="bi bi-${icon}"></i></div><div><strong>${label}</strong><small>${t.data?dateFmt.format(new Date(t.data)):'Data não informada'} • Taxa ${money.format(Number(t.taxa||0))}</small></div><strong>${sinal}${money.format(Number(t.valor||0))}</strong></div>`}).join('')}catch(e){toast('Erro ao carregar extrato',e.message)}}
async function remover(id){if(!confirm(`Excluir a conta #${id}?`))return;try{await request(`${api}/contas/${id}`,{method:'DELETE',headers:headers()});await listarContas();toast('Conta excluída','A conta foi removida com sucesso.')}catch(e){toast('Erro ao excluir conta',e.message)}}
async function transferir(){try{const origem=Number($('transferenciaOrigem').value);const contaDestinoId=Number($('transferenciaDestino').value);const valor=converterMoedaParaNumero($('transferenciaValor').value);if(!origem||!contaDestinoId||valor<=0)throw new Error('Preencha origem, destino e valor.');await request(`${api}/contas/${origem}/transferir`,{method:'POST',headers:headers(),body:JSON.stringify({contaDestinoId,valor})});await listarContas();toast('Transferência realizada','O valor foi transferido com sucesso.')}catch(e){toast('Erro na transferência',e.message)}}
function aplicarMascaraNumeroCartao(valor){return String(valor||'').replace(/\D/g,'').slice(0,16).replace(/(.{4})/g,'$1 ').trim()}
async function cadastrarCartao(event){event?.preventDefault();try{const c=await request(`${api}/cartoes/solicitar`,{method:'POST',headers:headers()});if($('cartaoPreview')){$('cartaoPreview').style.display='block';$('previewNumero').textContent=c.numero||'0000 0000 0000 0000';$('previewTitular').textContent=(c.nomeTitular||'NOME DO TITULAR').toUpperCase();$('previewValidade').textContent=c.validade||'00/00'}await listarCartoes();toast('Cartão solicitado','O back-end gerou número, validade, CVV e limite automaticamente.')}catch(e){toast('Erro ao solicitar cartão',e.message)}}
async function listarCartoes(){try{const cartoes=await request(`${api}/cartoes`,{headers:headers()})||[];const area=$('listaCartoes');if(!area)return;if(!cartoes.length){area.innerHTML='<div class="empty-state"><p>Nenhum cartão solicitado.</p></div>';return}area.innerHTML=cartoes.map(c=>`<article class="account-card"><div><h3>${c.nomeTitular}</h3><div class="account-meta"><span class="tag"><i class="bi bi-credit-card"></i>${c.numero||''}</span><span class="tag">Validade ${c.validade||''}</span><span class="tag">CVV ${c.cvv||''}</span></div><div class="account-actions"><button class="icon-btn" onclick="excluirCartao('${c.id}')"><i class="bi bi-trash"></i></button></div></div><div class="balance">${money.format(Number(c.limite||0))}</div></article>`).join('')}catch(e){toast('Erro ao listar cartões',e.message)}}
async function excluirCartao(id){if(!confirm('Deseja excluir este cartão?'))return;try{await request(`${api}/cartoes/${id}`,{method:'DELETE',headers:headers()});await listarCartoes();toast('Cartão excluído','O cartão foi removido com sucesso.')}catch(e){toast('Erro ao excluir cartão',e.message)}}

function iniciarCarrossel(){
  const carousel=document.querySelector('.carousel');
  if(!carousel)return;
  const slides=[...carousel.querySelectorAll('.slide')];
  const dotsArea=carousel.querySelector('.carousel-dots');
  const prev=carousel.querySelector('.prev');
  const next=carousel.querySelector('.next');
  let atual=Math.max(0,slides.findIndex(s=>s.classList.contains('active')));
  if(!slides.length)return;
  if(dotsArea){
    dotsArea.innerHTML=slides.map((_,i)=>`<button type="button" aria-label="Ir para slide ${i+1}"></button>`).join('');
  }
  const dots=[...(dotsArea?.querySelectorAll('button')||[])];
  function mostrar(i){
    atual=(i+slides.length)%slides.length;
    slides.forEach((s,idx)=>s.classList.toggle('active',idx===atual));
    dots.forEach((d,idx)=>d.classList.toggle('active',idx===atual));
  }
  prev?.addEventListener('click',()=>mostrar(atual-1));
  next?.addEventListener('click',()=>mostrar(atual+1));
  dots.forEach((d,i)=>d.addEventListener('click',()=>mostrar(i)));
  mostrar(atual);
}

document.addEventListener('DOMContentLoaded',async()=>{iniciarCarrossel();atualizarSessaoHome();$('loginForm')?.addEventListener('submit',login);$('registerForm')?.addEventListener('submit',registrar);$('contaForm')?.addEventListener('submit',criarConta);$('cartaoForm')?.addEventListener('submit',cadastrarCartao);$('actionSelect')?.addEventListener('change',e=>{if(e.target.value)window.location.href=e.target.value});document.querySelectorAll('.money-input,#saldoInicial,#valor,#transferenciaValor').forEach(c=>c.addEventListener('input',aplicarMascaraDinheiroInput));['transferenciaDestino'].forEach(id=>$(id)?.addEventListener('input',e=>e.target.value=e.target.value.replace(/\D/g,'')));if(document.body.dataset.page==='app'){protegerPagina();if(usuarioAtual){if($('welcomeName'))$('welcomeName').textContent=usuarioAtual.nome||'Cliente';if($('perfilNome'))$('perfilNome').textContent=usuarioAtual.nome||'Usuário';if($('perfilEmail'))$('perfilEmail').textContent=usuarioAtual.email||'email@cliente.com';if($('perfilTipo'))$('perfilTipo').textContent=usuarioAtual.perfil||'Cliente'}await listarContas();const params=new URLSearchParams(location.search);if(params.get('conta'))selecionarConta(params.get('conta'));if($('extrato')&&$('contaId')?.value)carregarExtrato(false);listarCartoes();}});


function atualizarNavbarUsuario(){
    const el = document.getElementById('navbarUser');
    if(!el) return;

    if(usuarioAtual && usuarioAtual.nome){
        el.innerText = `Olá ${usuarioAtual.nome}`;
    }else{
        el.innerText = 'Olá usuário';
    }
}

document.addEventListener('DOMContentLoaded', atualizarNavbarUsuario);

