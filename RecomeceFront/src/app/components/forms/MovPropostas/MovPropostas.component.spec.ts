/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MovPropostasComponent } from './MovPropostas.component';

describe('MovPropostasComponent', () => {
  let component: MovPropostasComponent;
  let fixture: ComponentFixture<MovPropostasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MovPropostasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MovPropostasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
