import { Injectable } from '@angular/core';

@Injectable()
export class CustomConvertsService {

    constructor() { }

    public FRetornaDataTempoCrm(DataP: any) {
        let Data = new Date(DataP);
        let Retorno = '';
        let DataAtual = new Date();
        let Ano = ` ${Data.getFullYear()}`;

        if (Data.getFullYear() !== DataAtual.getFullYear()) {
            Ano = ' de ' + Data.getFullYear();
        }

        if (Data.getHours() === 0) {
            Retorno = Data.getDate() + ' ' +
                ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'][Data.getMonth()];
        } else {
            Retorno = Data.getDate() + ' ' +
                ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'][Data.getMonth()] +
                Ano + ', ' +
                ('0' + Data.getHours()).slice(-2) + ':' + ('0' + Data.getMinutes()).slice(-2);
        }

        return Retorno;
    }

    public FFirtLastName(fullName: string): any {
        const nomes = fullName?.split(' ');
        if (!nomes) return;
        if (nomes[0] == nomes[nomes.length - 1])
            return nomes[0]
        else
            return `${nomes[0]} ${nomes[nomes.length - 1]}`
    }

    public padLeft(number: any, length: any) {
        return String(number).padStart(length, '0');
    }

    public DataFormatoBr(date: any): any {
        if (!date)
            return '';
        let dt = date.split("-");
        let x = dt[2].toString().substring(0, 2) + '/' + dt[1] + '/' + dt[0];
        if (x == '01/01/0001')
            return '';
        return x;
    }

    public DataTempoFormatoBr(date: any): any {
        try {
            let dt = date.split("-");
            let x = dt[2].substring(0, 2) + '/' + dt[1] + '/' + dt[0] + ' ' + dt[2].substring(3, 11);
            return x;
        }
        catch {
            return date;
        }
    }

    converterDecimalParaInt(valor: number): number {
        return Math.trunc(valor);
    }

    public DataFormatoUs(date: string): any {
        if (date == '' || !date)
            return '0001-01-01 00:00:00';
        let x = '';
        if (!date.includes('/')) {
            date = date.replace(' ', '');
            x = date.substring(8, 4) + '-' + date.substring(2, 4) + '-' + date.substring(0, 2)
        }
        else {
            let dt = date.split("/");
            if (dt)
                x = dt[2] + '-' + dt[1] + '-' + dt[0];
        }

        if (x == '--')
            return '0001-01-01 00:00:00';

        return x;
    }
    public ConverteDateUs(dt: Date): any {
        return dt.getFullYear() + '-' + this.padLeft(dt.getMonth(), 2) + '-' + this.padLeft(dt.getDate(), 2)
    }
    public ConverteDateBr(dt: Date): any {
        return `${this.padLeft(dt.getDate(), 2)}/${this.padLeft(dt.getMonth() + 1, 2)}/${dt.getFullYear()}`
    }
    public ConvertDateToDateTimeBr(dt: Date): any {
        return `${dt.getUTCDate().toString().padStart(2, '0')}/${(dt.getUTCMonth() + 1).toString().padStart(2, '0')}/${dt.getFullYear()} ${dt.getHours().toString().padStart(2, '0')}:${dt.getMinutes().toString().padStart(2, '0')}`;
    }
    public ConvertDataBrToUs(dt: string): any {
        const dts = dt.split(' ');
        const d = dts[0].split('/');
        return `${d[2]}-${d[1]}-${d[0]}`;
    }
    public ConvertDataTempoBrToUs(dt: string): any {
        const dts = dt.split(' ');
        const d = dts[0].split('/');
        return `${d[2]}-${d[1]}-${d[0]} ${dts[1]}`;
    }

    public AdicionarMeses = (date: any, m: number) => {
        const dt = new Date(date);
        dt.setMonth(m);
        return this.DataFormatoUs(dt.toString());
    }

    public MascaraDataHora(texto: string): string {
        //ngModelChange
        let v = texto;
        let l = texto.length;
        switch (l) {
            case 2:
                texto = v + "/";
                break;
            case 5:
                texto = v + "/";
                break;
            case 10:
                texto = v + " ";
                break;
            case 13:
                texto = v + ":";
                break;
            case 16:
                texto = v + "";
                break;
        }
        return texto;
    }

    public AdicionarMes = (date: any, m: number) => {
        let d = new Date(date);
        return new Date(d.setDate(-1));
    }

    public ConvertDataMascaraBrUs(e: any): any {
        if (e) {
            let x = e.split('/');
            if (x.length == 3)
                return x[2] + '-' + x[1] + '-' + x[0];
        }
        else {
            return '';
        }
    }

    public ConvertStringDecimal(v: string): number | string {
        let x: any = 0;
        x = parseFloat(v).toFixed(2);
        if (v)
            return x;
        else
            return 0;
    }

    public MascaraDecimal(v: any): any {
        if (v == null)
            return '';
        // let x = new Intl.NumberFormat('pt-BR').format(v);
        let x = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v);
        x = x.replace('R$', '').trim();
        return x;
    }
    public converterParaNumeroBR(valor: string): number {
        if (!valor) return 0;

        // Remove espaços e símbolo R$
        valor = valor.replace(/\s|R\$|\%/g, '');

        // Se tiver ponto e vírgula, é formato BR (ex: 1.234,56)
        if (valor.includes(','))
            valor = valor.replace(/\./g, '').replace(',', '.');
        else
            valor = valor.replace(',', '.'); // caso venha 4500,00 sem pontos

        const numero = parseFloat(valor);
        return isNaN(numero) ? 0 : numero;
    }

    public getInvariantValueFromMasked(value: string): number {
        if (!value) return 0;

        // Remove todos os caracteres não numéricos
        const digitsOnly = value.replace(/\D/g, '');

        if (digitsOnly.length === 0) return 0;

        // Parte inteira: tudo menos os 2 últimos dígitos
        const intPart = digitsOnly.slice(0, -2) || '0';

        // Parte decimal: os 2 últimos dígitos
        const decimalPart = digitsOnly.slice(-2);

        const result = parseFloat(`${intPart}.${decimalPart}`);

        return result;
    }
    public mascararPorcentagem(event: any): void {
        let valor = event.target.value;
        // Remove tudo que não for número ou vírgula/ponto
        valor = valor.replace(/[^\d,\.]/g, '');

        // Substitui vírgulas por pontos para normalizar
        valor = valor.replace(',', '.');

        // Converte para número
        let numero = parseFloat(valor);

        // Impede valores maiores que 100
        if (numero > 100) numero = 100;

        // Corrige para 2 casas decimais e converte para string com vírgula
        let valorFormatado = numero.toFixed(2).replace('.', ',');

        // Remove zeros desnecessários (ex: 50,00 → 50)
        valorFormatado = valorFormatado.replace(/,00$/, '');

        // Adiciona o símbolo de porcentagem
        event.target.value = valorFormatado + '%';
    }

    public mascararValorMonetario(event: any): void {
        let valor = event.target.value;

        // Remove tudo que não é número
        valor = valor.replace(/\D/g, '');

        // Converte para centavos e formata como BRL
        const valorNumerico = (parseInt(valor, 10) / 100).toFixed(2);

        // Formata com separador de milhar e vírgula decimal
        event.target.value = valorNumerico
            .replace('.', ',')
            .replace(/\B(?=(\d{3})+(?!\d))/g, '.');
    }

    public MascaraDecimalField(event: any, e: any, f: string) {
        if (event.target.value !== '') {
            if (event.target.value.length <= 2)
                return;
            let v = this.DecimalInvariant(event.target.value);
            let d = this.DecimalInput(v);
            e[f] = this.MascaraDecimal(this.DecimalInvariant(d));
        }
    }

    public MascaraReal(event: any) {
        // Remove todos os caracteres não numéricos
        let valor = event.target.value.replace(/\D/g, '');

        // Converte o valor para um número decimal
        valor = (parseFloat(valor) / 100).toFixed(2);

        // Formata o valor para o formato de moeda brasileira com separador de milhar
        valor = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(Number(valor));

        // Remove o símbolo da moeda para manter apenas o valor
        valor = valor.replace('R$', '').trim();

        // Atualiza o valor do event.target.value com a formatação correta
        event.target.value = valor;
    }

    public MascaraDecimalReactive(event: any) {
        let vl = event.target.value;
        if (vl !== '') {
            if (vl.length <= 2)
                return;
            let v = this.DecimalInvariant(vl);
            let d = this.DecimalInput(v);
            event.target.value = d;
        }
    }

    public DecimalBr(vo: any): any {
        let v = '' + vo + '';
        return v.replace('.', ',')
    }
    /*Função que padroniza valor monétario*/
    public DecimalInput(vo: any): any {
        let v = '' + vo + '';
        if (!v.replace) return v;
        v = v.replace(/\D/g, "") //Remove tudo o que não é dígito
        v = v.replace(/^([0-9]{3}\.?){3}-[0-9]{2}$/, "$1,$2");
        //v=v.replace(/(\d{3})(\d)/g,"$1,$2")
        v = v.replace(/(\d)(\d{2})$/, "$1,$2") //Coloca ponto antes dos 2 últimos digitos
        return v;
    }

    public DecimalInvariant(v: string): any {
        let vS = '' + v + '';
        let x = vS.replaceAll('.', '').replace(',', '.');
        return x;
    }

    public DecimalInvariantReactive(valor: string): any {
        valor = valor.toString().replace(/\D/g, '');
        let vx = valor.substring(0, valor.length - 2) + '.' + valor.substring(valor.length - 2);
        return vx;
    }

    public LimparCaracteres(t: string): string {
        if (!t) return '';
        t = t?.replace("-", "").replace(".", "").replace("/", "").replace("_", "").replace("(", "").replace(")", "").replace(".", "").replace("-", "").replace("+", "").trim();
        return t;
    }

    public MascaraTelefone(v: any): string {
        if (!v)
            return '';
        var r = v.replace(/\D/g, "");
        r = r.replace(/^0/, "");
        if (r.length > 10) {
            r = r.replace(/^(\d\d)(\d{5})(\d{4}).*/, "($1) $2-$3");
        } else if (r.length > 5) {
            r = r.replace(/^(\d\d)(\d{4})(\d{0,4}).*/, "($1) $2-$3");
        } else if (r.length > 2) {
            r = r.replace(/^(\d\d)(\d{0,5})/, "($1) $2");
        } else {
            r = r.replace(/^(\d*)/, "($1");
        }
        return r;
    }

    public RetornaWppBotao(t: string): string {
        let c = this.LimparCaracteres(t);
        let lb = " <a href=\"https://api.whatsapp.com/send?phone=+55" + c + "\" target=\"_blank\">" + this.MascaraTelefone(t) + "</a>";
        lb = "<i class=\"fab fa-whatsapp text-success mr-1\"></i> " + lb;

        return lb;
    }

    public ConvertEmString(t: any): string {
        return '' + t + '';
    }

    public ArquivosAceitos(b: string): any {
        let map = new Map<string, string>();
        map.set("text/plain", ".txt");
        map.set("text/xml", ".xml");
        map.set("application/pdf", ".pdf");
        map.set("application/vnd.ms-word", ".doc");
        map.set("application/vnd.ms-word", ".docx");
        map.set("application/vnd.ms-excel", ".xls");
        map.set("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx");
        map.set("image/png", ".png");
        map.set("image/jpeg", ".jpg");
        map.set("image/jpeg", ".jpeg");
        map.set("image/gif", ".gif");
        map.set("text/csv", ".csv");
        map.set("application/zip", ".zip");

        return map.get(b);
    }

    public DocumentosExtensoes(): any {
        let map = [];
        map.push("application/zip");
        map.push("text/plain");
        map.push("text/xml");
        map.push("application/pdf");
        map.push("application/vnd.ms-word");
        map.push("application/vnd.ms-word");
        map.push("application/vnd.ms-excel");
        map.push("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        return map;
    }

    public MascaraCnpjCpf(s: any): string {
        if (!s) return 'vazio';

        if (s.length == 15) { // Completado com 0 a esquerda
            s = s.substring(1, 15);

        }
        if (s.length == 14) {
            s = s.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, "$1.$2.$3/$4-$5")
            return s;
        }
        else if (s.length == 11) {
            s = s.replace(/^(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4")
            return s;
        }
        return s;
    }

    public MascaraCEP(valor: string): string {
        const raw = valor.replace(/\D/g, '');
        // Aplica a máscara 99999-999
        const masked = raw.length <= 5
            ? raw
            : `${raw.substring(0, 5)}-${raw.substring(5, 8)}`;

        return masked;
    }

    public PrimeiraPaginaGrid(pags: any): any {
        pags.paginaAtual = 1;
        return pags;
    }

    public RetornaDataTempoBrTimeStamp(s: any, t: number = 0): any {
        s = +s;
        let dt = new Date(s);
        if (dt.toString() == 'Invalid Date') {
            return '';
        }
        let tempo = '';
        if (t == 0)
            tempo = `${dt.getHours()}:${dt.getMinutes()}:${dt.getSeconds()}`;
        else if (t == 1)
            tempo = `${dt.getHours()}:${dt.getMinutes()}`;

        let dataBr = `${dt.getDate()}/${(dt.getMonth() + 1)}/${dt.getFullYear()}`
        let obj = {
            Data: dataBr,
            Tempo: tempo
        }
        return obj;
    }

    public ListaBancos(): any {
        let bancos = [];
        bancos.push({ CodigoBanco: 'XXX', NomeBanco: 'Escolha um banco...' })
        bancos.push({ CodigoBanco: '000', NomeBanco: 'Caixa Empresa' })
        bancos.push({ CodigoBanco: '403', NomeBanco: 'Banco Cora' })
        bancos.push({ CodigoBanco: '001', NomeBanco: 'Banco do Brasil S.A.' })
        bancos.push({ CodigoBanco: '104', NomeBanco: 'Caixa Econômica Federal' })
        bancos.push({ CodigoBanco: '237', NomeBanco: 'Banco Bradesco S.A.' })
        bancos.push({ CodigoBanco: '260', NomeBanco: 'Nu Pagamentos S.A (Nubank)' })
        bancos.push({ CodigoBanco: '336', NomeBanco: 'Banco C6 S.A – C6 Bank' })
        bancos.push({ CodigoBanco: '756', NomeBanco: 'Banco Cooperativo do Brasil S.A. – BANCOOB' })
        bancos.push({ CodigoBanco: '748', NomeBanco: 'Banco Cooperativo Sicredi S.A.' })
        bancos.push({ CodigoBanco: '069', NomeBanco: 'Banco Crefisa S.A.' })
        bancos.push({ CodigoBanco: '077', NomeBanco: 'Banco Inter S.A.' })
        bancos.push({ CodigoBanco: '184', NomeBanco: 'Banco Itaú BBA S.A.' })
        bancos.push({ CodigoBanco: '479', NomeBanco: 'Banco ItauBank S.A' })
        bancos.push({ CodigoBanco: '074', NomeBanco: 'Banco J. Safra S.A.' })
        bancos.push({ CodigoBanco: '389', NomeBanco: 'Banco Mercantil do Brasil S.A.' })
        bancos.push({ CodigoBanco: '212', NomeBanco: 'Banco Original S.A.' })
        bancos.push({ CodigoBanco: '422', NomeBanco: 'Banco Safra S.A.' })
        bancos.push({ CodigoBanco: '033', NomeBanco: 'Banco Santander (Brasil) S.A.' })
        bancos.push({ CodigoBanco: '348', NomeBanco: 'Banco Xp S/A' })
        bancos.push({ CodigoBanco: '341', NomeBanco: 'Itaú Unibanco S.A.' })
        bancos.push({ CodigoBanco: '102', NomeBanco: 'Xp Investimentos S.A' })

        return bancos;
    }
    public BuscaBancoCodigo(cod: string): string {
        const banco = this.ListaBancos().find((b: any) => b.CodigoBanco === cod);
        return banco ? banco.NomeBanco : '';
    }
    public SimNaoInt(valor: number): string {
        return valor == 1 ? 'Sim' : 'Não';
    }
    public VerificaExistenciaObjetoComplexo(obj: any, f: any, v: any): boolean {

        let result = obj.filter(function (el: any) {
            return el[f] == v;
        });

        if (result.length)
            return true;

        return false;
    }

    public formatXml(xml: any, tab: any) { // tab = optional indent value, default is tab (\t)
        let formatted = '', indent = '';
        tab = tab || '\t';
        xml.split(/>\s*</).forEach(function (node: any) {
            if (node.match(/^\/\w/)) indent = indent.substring(tab.length); // decrease indent by one 'tab'
            formatted += indent + '<' + node + '>\r\n';
            if (node.match(/^<?\w[^>]*[^\/]$/)) indent += tab;              // increase indent
        });
        return formatted.substring(1, formatted.length - 3);
    }

    public RetornaValorElementoId(id: string): any {
        let el: any = document.getElementById(id);
        if (el)
            return el.value;
        return ''
    }
    public BuscaObjetoPorCodigo(obj: any, field: string, id: any) {
        return obj.filter(function (el: any) {
            return el[field] == id;
        });
    }
    public AtualizarGrid() {
        // const btn = document.getElementById('BtnMontarGrid');
        // btn?.click();
        this.ClicarBotaoNativoId('BtnMontarGrid')
    }

    public ClicarBotaoNativoId(id: any): void {
        const btn = document.getElementById(id);
        btn?.click();
    }
    public CarregaTotalizadorMenu(): void {
        this.ClicarBotaoNativoId('BtnAtualizaNotificacoes');
    }
    public ToogleBackDrop(): void {
        this.ClicarBotaoNativoId('BtnBackDrop')
    }
    public BuscaIndicePorCodigo(obj: any, field: string, id: any) {
        let indice = obj.indexOf(obj.filter(function (el: any) {
            return el[field] == id;
        })[0]);
        return indice;
    }

    public OrdenarObjeto(r: any, ord: any): any {
        r = r.sort(function (a: any, b: any) {
            return a[ord] < b[ord] ? -1 : a[ord] > b[ord] ? 1 : 0;
        });
    }
    public isBoolInt(valor: any): boolean {
        if (valor == '1')
            return true;
        else
            return false;
    }
    public DropdownTodos(data: any): any {
        return [{ Valor: '0', Texto: 'Todos' }, ...data]
    }
    public RetornaCamposEditorHTML(): any {
        return [
            {
                Campo: 'Nome',
                Coluna: 'Nome'
            },
            {
                Campo: 'E-Mail',
                Coluna: 'Email'
            },
            {
                Campo: 'Telefone',
                Coluna: 'Telefone'
            },
            {
                Campo: 'Valor',
                Coluna: 'Valor'
            },
            {
                Campo: 'Meu Cnpj',
                Coluna: 'MeuCnpj'
            },
            {
                Campo: 'Minha Razão Social',
                Coluna: 'MinhaRazSoc'
            },
            {
                Campo: 'Telefone Empresarial',
                Coluna: 'MeuTelefoneEmpresarial'
            },
            {
                Campo: 'E-Mail Empresarial',
                Coluna: 'MeuEmailEmpresarial'
            },
            {
                Campo: 'Meu E-Mail',
                Coluna: 'MeuEmail'
            },
            {
                Campo: 'Meu Nome',
                Coluna: 'MeuNome'
            },
            {
                Campo: 'Meu Telefone',
                Coluna: 'MeuTelefone'
            },
            {
                Campo: 'Assinatura E-mail',
                Coluna: 'MinhaAssinatura'
            },
        ]
    }

    public formatarDateUsIso(date: Date): any {
        const ano = date.getFullYear();
        const mes = String(date.getMonth() + 1).padStart(2, '0'); // adiciona zero à esquerda, se necessário
        const dia = String(date.getDate()).padStart(2, '0'); // adiciona zero à esquerda, se necessário
        return `${ano}-${mes}-${dia}`;
    }
    public getDataInicialEFinalDoMes(): any {
        const dataAtual = new Date();
        const primeiroDiaDoMes = new Date(dataAtual.getFullYear(), dataAtual.getMonth(), 1);
        const ultimoDiaDoMes = new Date(dataAtual.getFullYear(), dataAtual.getMonth() + 1, 0);

        return {
            dataInicial: this.formatarDateUsIso(primeiroDiaDoMes),
            dataFinal: this.formatarDateUsIso(ultimoDiaDoMes)
        };
    }

    public LoaderToogle(): void {
        const loader = document.getElementById('loader');
        if (loader) {
            if (loader.className == 'full-loader') {
                loader.classList.add('show')
            }
            else {
                loader.classList.remove('show')
            }
        }
    }

    public ValidaDataBr(valor: any): boolean {
        let s = valor.split('/');
        const dt = `${s[2].substring(0, 4)}-${s[1]}-${s[0]}`
        try {
            const data: any = new Date(dt);
            if (data == 'Invalid Date')
                return false;
        }
        catch {
            return false;
        }
        return true;
    }

    public ValidaDataUs(valor: any): boolean {
        let s = valor.split('-');
        const dt = `${s[2].substring(0, 4)}-${s[1]}-${s[0]} 10:00`
        try {
            const data: any = new Date(dt);
            if (data == 'Invalid Date')
                return false;
        }
        catch {
            return false;
        }
        return true;
    }

    public ValidaDataMenor(dateToCheck: Date, days: number): boolean {
        const currentDate = new Date();
        const later = new Date(currentDate.getTime() + days * 24 * 60 * 60 * 1000);
        return dateToCheck < later;
    }
    public ZeroEsquerda(valor: any): any {
        return valor.toString().padStart(2, '0');
    }
    public validarCpfCnpj(insc: string): boolean {
        if (!insc) return false;
        insc = insc.replace(/\D/g, '');
        if (insc.length == 11)
            return this.validarCpf(insc);
        else
            return this.validarCNPJ(insc);
    }
    public validarCpf(cpf: string): boolean {
        if (!cpf) return false;

        // Remove caracteres não numéricos
        cpf = cpf.replace(/\D/g, '');

        if (cpf.length !== 11) return false;

        // Elimina CPFs inválidos conhecidos
        if (/^(\d)\1+$/.test(cpf)) return false;

        let soma = 0;
        let resto;

        // Valida primeiro dígito
        for (let i = 1; i <= 9; i++) {
            soma += parseInt(cpf.charAt(i - 1)) * (11 - i);
        }
        resto = (soma * 10) % 11;
        if (resto === 10 || resto === 11) resto = 0;
        if (resto !== parseInt(cpf.charAt(9))) return false;

        // Valida segundo dígito
        soma = 0;
        for (let i = 1; i <= 10; i++) {
            soma += parseInt(cpf.charAt(i - 1)) * (12 - i);
        }
        resto = (soma * 10) % 11;
        if (resto === 10 || resto === 11) resto = 0;
        if (resto !== parseInt(cpf.charAt(10))) return false;

        return true;
    }
    public validarCNPJ(cnpj: any): boolean {
        // Remove todos os caracteres que não sejam números
        cnpj = cnpj.replace(/[^\d]+/g, '');

        if (cnpj == '') return false;

        // CNPJ tem que ter 14 dígitos
        if (cnpj.length !== 14)
            return false;

        // Elimina CNPJs conhecidos que são inválidos
        if (cnpj == "00000000000000" ||
            cnpj == "11111111111111" ||
            cnpj == "22222222222222" ||
            cnpj == "33333333333333" ||
            cnpj == "44444444444444" ||
            cnpj == "55555555555555" ||
            cnpj == "66666666666666" ||
            cnpj == "77777777777777" ||
            cnpj == "88888888888888" ||
            cnpj == "99999999999999")
            return false;

        // Validação do primeiro dígito verificador
        let tamanho = cnpj.length - 2;
        let numeros = cnpj.substring(0, tamanho);
        let digitos = cnpj.substring(tamanho);
        let soma = 0;
        let pos = tamanho - 7;
        for (let i = tamanho; i >= 1; i--) {
            soma += numeros.charAt(tamanho - i) * pos--;
            if (pos < 2)
                pos = 9;
        }
        let resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(0))
            return false;

        // Validação do segundo dígito verificador
        tamanho = tamanho + 1;
        numeros = cnpj.substring(0, tamanho);
        soma = 0;
        pos = tamanho - 7;
        for (let i = tamanho; i >= 1; i--) {
            soma += numeros.charAt(tamanho - i) * pos--;
            if (pos < 2)
                pos = 9;
        }
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(1))
            return false;

        return true;
    }

    MaskCEP(ev: any): void {
        // Remove tudo que não for número
        const raw = ev.target.value.replace(/\D/g, '');

        // Aplica a máscara 99999-999
        const masked = raw.length <= 5
            ? raw
            : `${raw.substring(0, 5)}-${raw.substring(5, 8)}`;

        console.log(masked);

        ev.target.value = masked;
    }
    public isDateValid(dateString: string): boolean {
        const datePattern = /^(\d{2})\/(\d{2})\/(\d{4})$/;

        // Verifica se a string corresponde ao padrão dd/mm/yyyy
        const match = datePattern.exec(dateString);
        if (!match) {
            return false;
        }

        const day = parseInt(match[1], 10);
        const month = parseInt(match[2], 10);
        const year = parseInt(match[3], 10);

        // Verifica se a data é válida
        const date = new Date(year, month - 1, day);
        if (
            date.getFullYear() !== year ||
            date.getMonth() !== month - 1 ||
            date.getDate() !== day
        ) {
            return false;
        }

        return true; // A data é válida
    }
    public RetornaDataAtualBr(): any {
        return this.ConverteDateBr(new Date());
    }
    public RetornaClasseAtivo(tipo: any): any {
        return tipo == 1 ? 'aprovado' : 'recusado';
    }
    public RetornaTextoAtivo(tipo: any): any {
        return tipo == 1 ? 'Ativo' : 'Inativo';
    }
    public OpenNewTab(path: any): void {
        const baseUrl = window.location.origin;
        const url = `${baseUrl}/${path}`;
        window.open(url, '_blank');
    }
}
