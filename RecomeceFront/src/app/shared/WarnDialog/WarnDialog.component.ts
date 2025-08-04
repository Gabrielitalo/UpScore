import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-WarnDialog',
  templateUrl: './WarnDialog.component.html',
  styleUrls: ['./WarnDialog.component.css']
})
export class WarnDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
  private dialogRef: MatDialogRef<WarnDialogComponent>,) { }

  ngOnInit() {
  }

  onAction(type: boolean): void {
    this.dialogRef.close({ confirmed: type });
  }
  

}
