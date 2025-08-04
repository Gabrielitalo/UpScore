/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { ViewBeneficiariosLoteDialogComponent } from './ViewBeneficiariosLoteDialog.component';

describe('ViewBeneficiariosLoteDialogComponent', () => {
  let component: ViewBeneficiariosLoteDialogComponent;
  let fixture: ComponentFixture<ViewBeneficiariosLoteDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewBeneficiariosLoteDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewBeneficiariosLoteDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
