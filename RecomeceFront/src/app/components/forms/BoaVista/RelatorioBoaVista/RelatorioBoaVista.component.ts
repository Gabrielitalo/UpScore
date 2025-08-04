import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../../services/Auth.service';
import { XmlParserService } from '../../../../services/XmlParser.service';
import printJS from 'print-js';
declare var html2pdf: any;
import html2canvas from 'html2canvas';
import { LoaderService } from '../../../../services/loader.service';

@Component({
    selector: 'app-RelatorioBoaVista',
    templateUrl: './RelatorioBoaVista.component.html',
    styleUrls: ['./RelatorioBoaVista.component.css']
})
export class RelatorioBoaVistaComponent implements OnInit {

    constructor(private request: RequestsService,
        public cc: CustomConvertsService,
        private loader: LoaderService,
        public router: Router,
        private route: ActivatedRoute,
        private auth: AuthService,
        private xmlJson: XmlParserService,
    ) {
        const idParam = this.route.snapshot.paramMap.get('id');
        this.id = idParam ? idParam : '';
    }

    private id = '';
    public reportData: any;

    ngOnInit() {
        this.getReport();
    }

    public Back(): void {
        this.router.navigate(['Consultas']);
    }

    private async getReport(): Promise<void> {
        const response = await this.request.getData(`/LogConsultas/${this.id}`);
        if (response) {
            const d = this.xmlJson.xmlToJson(response.arquivoRetorno)
            // console.log(d);

            this.reportData = response;
            this.reportData.arquivoRetorno = d;
            this.reportData.personType = this.reportData.inscricao.length == 11 ? 1 : 2;

            // this.reportData = JSON.parse(response.arquivoRetorno)
            // console.log(this.reportData);
        }
    }

    print2() {
        const element = document.getElementById('printArea');
        if (!element) return;

        const options = {
            margin: 2,
            filename: 'relatorio.pdf',
            autoPaging: 'text',
            image: { type: 'jpeg', quality: 1 },
            html2canvas: {
                scale: 2, useCORS: true, scrollX: 0, scrollY: -window.scrollY, logging: false
            },
            jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
        };

        html2pdf().from(element).set(options).save();


    }

    async print() {
        this.loader.toogleLoader();
        const conteudo = document.getElementById('printArea')?.innerHTML;
        const estilo = `
        <html>
        <head>
        <style> 
        :host {
            --blue: #3c6496;
            --leftBorder: #3e6499;
            --card-bg: #ECEEF4;
            --textColor: #424242;
            --textValueColor: #9b9b9b;
            --bg: #F8F9FA;
            --red: #e53e3e;
            --green: #38a169;
            --orange: #e17f11;
        }
        
        .report-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
        }
        
        .report-header img {
            width: 120px;
            height: 60px;
        }
        
        .a4-page {
            /* width: 210mm;
            height: 297mm; */
            padding: 0;
            box-sizing: border-box;
            background-color: #F8F9FA;
            page-break-after: always;
            width: 794px;
        }
        
        .report-tittle {
            color: #3c6496;
            font-weight: 500;
            font-size: 18px;
        }
        
        
        .item-container {
            display: flex;
            flex-direction: column;
        }
        
        .item-container p {
            margin: 5px;
        }
        
        .item-header {
            display: flex;
            border-left: 5px solid #3e6499;
            padding-left: 10px;
            color: #3c6496;
            background-color: #ECEEF4;
            height: 40px;
            align-items: center;
            margin-bottom: 10px;
        }
        
        .item-row {
            display: flex;
            align-items: center;
            background-color: #fff;
        }
        
        .item-25 {
            width: 20%;
        }
        
        .item-25 {
            width: 25%;
        }
        
        .item-33 {
            width: 33.33333333333333%;
        }
        
        .item-40 {
            width: 40%;
        }
        
        .item-50 {
            width: 50%;
        }
        
        .item-75 {
            width: 75%;
        }
        
        .item-100 {
            width: 100%;
        }
        
        
        .item {
            font-size: 11px;
            color: #9b9b9b;
        }
        
        .item p:first-child {
            font-weight: 600;
            color: #424242;
        }
        
        .white-card {
            display: flex;
            background-color: #fff;
            padding: 15px;
            border-radius: 10px;
            font-size: 13px;
        }
        
        .white-card-border {
            display: flex;
            background-color: #fff;
            padding: 15px;
            border-radius: 10px;
            font-size: 13px;
            border: 1px solid #e1e1e1;
            align-items: center;
        }
        
        
        .icon-green {
            color: #38a169 !important;
            filter: invert(47%) sepia(99%) saturate(336%) hue-rotate(78deg) brightness(92%) contrast(88%);
        }
        
        .icon-red {
            color: #e53e3e !important;
            filter: invert(24%) sepia(99%) saturate(7497%) hue-rotate(357deg) brightness(93%) contrast(116%);
        }
        
        .icon-renda{
            filter: brightness(0) saturate(100%) invert(12%) sepia(72%) saturate(3034%) hue-rotate(202deg) brightness(102%) contrast(103%);
        }
        
        .color-renda{
            color: #0f54a9 !important;
        }
        
        .header-avulso {
            display: flex;
            font-size: 15px;
            margin: 5px;
            height: 40px;
            align-items: center;
        }
        
        
        .gauge-container {
            width: 300px;
            margin: auto;
            text-align: center;
            position: relative;
        }
        
        .gauge {
            width: 100%;
            height: auto;
        }
        
        .arc {
            fill: none;
            stroke: #59c96e;
            stroke-width: 16;
            stroke-linecap: round;
        }
        
        .arc-orange {
            fill: none;
            stroke: #f9b226;
            stroke-width: 16;
            stroke-linecap: round;
        }
        
        .gauge-title {
            margin-top: 10px !important;
            font-weight: 600;
            font-size: 12px;
        }
        
        .gauge-value {
            font-size: 36px;
            font-weight: bold;
            color: #222;
            position: absolute;
            top: 45%;
            left: 50%;
            transform: translate(-50%, -50%);
        }
        
        .gauge-labels {
            display: flex;
            justify-content: space-between;
            font-size: 14px;
            margin-top: 10px;
        }
        
        .score-container {
            display: flex;
            background-color: #fff;
            margin-top: 20px;
            padding: 15px;
            gap: 50px;
        }
        
        .image-boa {
            width: 100px;
            height: 100px;
        }
        
        .score-info {
            font-size: 11px;
            color: #9b9b9b;
            text-align: center;
        }
        
        .table-header {
            background-color: #ebebeb;
            height: 40px;
            font-weight: 600;
            font-size: 12px;
            text-align: center;
        }
        
        .table-body tr {
            text-align: center;
            font-size: 11px;
            height: 40px;
            color: #9b9b9b;
        }
        
        .table-body-black tr {
            text-align: center;
            font-size: 11px;
            height: 40px;
            color: #414141;
        }
        
        .forcar-quebra {
            page-break-before: always;
            /* força início de nova página */
        }
        
        table {
            width: 100%;
            border-collapse: collapse;
            page-break-inside: auto;
            background-color: #fff;
        }
        
        thead {
            display: table-header-group;
            /* garante que o cabeçalho apareça em cada página */
        }
        
        tr {
            page-break-inside: avoid;
            /* evita quebras dentro de uma linha */
            page-break-after: auto;
        }
        
        td,
        th {
            padding: 6px 8px;
            font-size: 12px;
            word-break: break-word;
        }
        
        .page-break-container {
            page-break-inside: auto;
            break-inside: auto;
        }
        
        #printArea {
            display: block;
        }
        
        .table-title{
            font-size: 12px;
            font-weight: 600;
        }
        
        .nada-consta{
            font-size: 11px;
            font-weight: 600;
            color: #686868;
        }
       </style>
       <head>`;
        // printJS({
        //     printable: estilo + conteudo,
        //     type: 'raw-html'
        // });

        const content = estilo + conteudo;

        const obj = {
            fileName: `${estilo}<body>${conteudo} </body></html>`
        }
        const response = await this.request.postDataBlob(`/LogConsultas/GerarPdf`, obj)
        console.log(response);
        this.loader.toogleLoader();
        const link = document.createElement('a');
        link.href = URL.createObjectURL(response);
        const agora = new Date();
        const pad = (n: any) => n.toString().padStart(2, '0');
        const nomeArquivo = `relatorio.pdf`;
        link.download = nomeArquivo;
        link.click();
        URL.revokeObjectURL(link.href); // limpa a memória
    }


}
