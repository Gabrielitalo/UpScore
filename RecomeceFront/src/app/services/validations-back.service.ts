import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ValidationsDialogComponent } from '../shared/ValidationsDialog/ValidationsDialog.component';

@Injectable({
  providedIn: 'root'
})
export class ValidationsBackService {

  constructor(private dialog: MatDialog,) { }

  openDialog(content: any): void {
    this.dialog.open(ValidationsDialogComponent, {
      width: '768px',
      data: content,
    });
  }
  
}
