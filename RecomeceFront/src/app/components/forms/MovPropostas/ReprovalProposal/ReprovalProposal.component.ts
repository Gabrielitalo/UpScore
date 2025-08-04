import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RequestsService } from '../../../../services/Requests.service';

@Component({
  selector: 'app-ReprovalProposal',
  templateUrl: './ReprovalProposal.component.html',
  styleUrls: ['./ReprovalProposal.component.css']
})
export class ReprovalProposalComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<ReprovalProposalComponent>,
    private request: RequestsService) { }

  ngOnInit() {
    // console.log(this.data);
  }

  async onConfirm(): Promise<void> {
    if (!this.data.proposalId)
      return;
    if (this.data.tipo == 'proposta')
      await this.request.getData(`/MovPropostas/UpdateStatus?proposalId=${this.data.proposalId}&status=4`)
    else if (this.data.tipo == 'contrato')
      await this.request.getData(`/MovContratos/UpdateStatus?contractId=${this.data.proposalId}&status=4`)

    window.location.reload();
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
