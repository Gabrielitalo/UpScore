/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { ChequesComponent } from './Cheques.component';

describe('ChequesComponent', () => {
  let component: ChequesComponent;
  let fixture: ComponentFixture<ChequesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChequesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChequesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
