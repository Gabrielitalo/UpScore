import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-ValidationsDialog',
  templateUrl: './ValidationsDialog.component.html',
  styleUrls: ['./ValidationsDialog.component.css']
})
export class ValidationsDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
  private dialogRef: MatDialogRef<ValidationsDialogComponent>,) { }

  ngOnInit() {
  }

  public Close(): void {
    this.dialogRef.close();
  }
}
