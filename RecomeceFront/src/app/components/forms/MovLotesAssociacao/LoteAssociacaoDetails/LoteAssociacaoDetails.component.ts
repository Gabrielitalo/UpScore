import { Component, Inject, OnInit } from '@angular/core';
import { AuthService } from '../../../../services/Auth.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { RequestsService } from '../../../../services/Requests.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';

@Component({
  selector: 'app-LoteAssociacaoDetails',
  templateUrl: './LoteAssociacaoDetails.component.html',
  styleUrls: ['./LoteAssociacaoDetails.component.css']
})
export class LoteAssociacaoDetailsComponent implements OnInit {

  constructor(private auth: AuthService,
    public snack: SnackBarService,
    private request: RequestsService,
    private dialogRef: MatDialogRef<LoteAssociacaoDetailsComponent>,
    public cc: CustomConvertsService,
    @Inject(MAT_DIALOG_DATA) public data: { id: any }
  ) { }

  public childBatch: any;

  ngOnInit() {
  }

  public receiveData(ev: any): void {
    console.log(ev);
    this.childBatch = ev;
  }

}
