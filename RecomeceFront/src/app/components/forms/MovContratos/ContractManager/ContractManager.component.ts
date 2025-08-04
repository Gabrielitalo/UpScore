import {
  Component,
  ElementRef,
  Inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RequestsService } from '../../../../services/Requests.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { LoaderService } from '../../../../services/loader.service';
import { AuthService } from '../../../../services/Auth.service';

@Component({
  selector: 'app-ContractManager',
  templateUrl: './ContractManager.component.html',
  styleUrls: ['./ContractManager.component.css'],
})
export class ContractManagerComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public proposal: any,
    private dialogRef: MatDialogRef<ContractManagerComponent>,
    public cc: CustomConvertsService,
    private loader: LoaderService,
    public auth: AuthService,
    public snack: SnackBarService,
    private request: RequestsService
  ) {}

  public fichas: any;
  public contract: any;
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  selectedFile: File | null = null;
  private beneficiarioId = 1;
  ngOnInit() {
    this.getFichas();
  }

  private async getFichas(): Promise<void> {
    const response = await this.request.getData(
      `/MovPropostasArquivos/GetAllProposal?proposal=${this.proposal.fk_MovPropostas}`
    );
    this.fichas = response;
    this.getContract();

    // console.log(this.fichas);
  }

  public async getContract(): Promise<void> {
    const response = await this.request.getData(
      `/MovPropostasArquivos/GetContractProposal?proposal=${this.proposal.fk_MovPropostas}`
    );
    this.contract = response;
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    const reader = new FileReader();

    reader.onload = () => {
      const base64 = (reader.result as string).split(',')[1];
      const payload = {
        base64: base64,
        name: file.name,
      };
      this.upload(payload);
    };

    reader.readAsDataURL(file);
  }

  public async upload(payload: any): Promise<void> {
    this.loader.toogleLoader();
    const model = {
      movPropostas: { id: this.proposal.fk_MovPropostas },
      MovPropostasBeneficiarios: { id: this.beneficiarioId },
      nomeArquivo: payload.name,
      arquivoBytes: payload.base64,
      tipo: this.beneficiarioId == 0 ? 1 : 2,
    };
    const response = await this.request.postData(
      `/MovPropostasArquivos`,
      model
    );
    this.loader.toogleLoader();
    this.snack.CreateNotification('Arquivo enviado com sucesso');
    this.getFichas();
  }

  selectFile(id: any) {
    this.beneficiarioId = id;
    this.fileInput.nativeElement.value = ''; // limpa o valor anterior
    this.fileInput.nativeElement.click(); // abre seletor de arquivos
  }

  public async deleteFile(id: any): Promise<void> {
    this.loader.toogleLoader();
    this.request.deleteData(`/MovPropostasArquivos/${id}`);
    await this.getContract();
    this.loader.toogleLoader();
  }

  public async downloadFile(id: any): Promise<void> {
    this.loader.toogleLoader();
    const response = await this.request.downloadData(`/MovPropostasArquivos/DownloadArquivo?id=${id}`);
    this.loader.toogleLoader();
    const link = document.createElement('a');
    link.href = URL.createObjectURL(response);
    const agora = new Date();
    const pad = (n: any) => n.toString().padStart(2, '0');
    const nomeArquivo = `ContratoAssinado.pdf`;
    link.download = nomeArquivo;
    link.click();
    URL.revokeObjectURL(link.href); // limpa a memória
  }

  public async downloadAssociativePdf(): Promise<void> {
    this.loader.toogleLoader();
    const response = await this.request.downloadData(
      `/MovContratos/DownloadFichasAssociativas?proposalId=${this.proposal.fk_MovPropostas}`
    );
    this.loader.toogleLoader();
    const link = document.createElement('a');
    link.href = URL.createObjectURL(response);
    const agora = new Date();
    const pad = (n: any) => n.toString().padStart(2, '0');
    const nomeArquivo = `FichasAssociativas_${agora.getFullYear()}-${pad(
      agora.getMonth() + 1
    )}-${pad(agora.getDate())}_${pad(agora.getHours())}-${pad(
      agora.getMinutes()
    )}-${pad(agora.getSeconds())}.zip`;
    link.download = nomeArquivo;
    link.click();
    URL.revokeObjectURL(link.href); // limpa a memória
  }

  public async downloadContract(): Promise<void> {
    this.loader.toogleLoader();
    const response = await this.request.downloadData(
      `/MovContratos/DownloadContrato?contractId=${this.proposal.id}`
    );
    this.loader.toogleLoader();
    const link = document.createElement('a');
    link.href = URL.createObjectURL(response);
    const agora = new Date();
    const pad = (n: any) => n.toString().padStart(2, '0');
    const nomeArquivo = `ContratoAssinado.pdf`;
    link.download = nomeArquivo;
    link.click();
    URL.revokeObjectURL(link.href); // limpa a memória
  }
}
