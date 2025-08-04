import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RequestsService } from '../../../../services/Requests.service';

@Component({
  selector: 'app-AprovalProposal',
  templateUrl: './AprovalProposal.component.html',
  styleUrls: ['./AprovalProposal.component.css']
})
export class AprovalProposalComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public proposal: any,
    private dialogRef: MatDialogRef<AprovalProposalComponent>,
    private request: RequestsService) { }


  ngOnInit() {
    // console.log(this.proposal);
  }

  async onConfirm(): Promise<void> {
    // const respose = await this.request.getData(`/MovPropostas/UpdateStatus?proposalId=${this.proposalId}&status=4`)
    // window.location.reload();
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  async receiveClient(event: any): Promise<void> {
    if (event == true) {
      await this.request.getData(`/MovPropostas/UpdateStatus?proposalId=${this.proposal.id}&status=3`)
      window.location.reload();
    }

  }
}
