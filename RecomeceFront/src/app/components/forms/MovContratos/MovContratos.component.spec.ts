/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MovContratosComponent } from './MovContratos.component';

describe('MovContratosComponent', () => {
  let component: MovContratosComponent;
  let fixture: ComponentFixture<MovContratosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MovContratosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MovContratosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
