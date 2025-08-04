/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { RendaEstimadaComponent } from './RendaEstimada.component';

describe('RendaEstimadaComponent', () => {
  let component: RendaEstimadaComponent;
  let fixture: ComponentFixture<RendaEstimadaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RendaEstimadaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RendaEstimadaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
