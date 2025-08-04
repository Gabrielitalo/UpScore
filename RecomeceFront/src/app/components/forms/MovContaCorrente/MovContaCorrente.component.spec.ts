/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MovContaCorrenteComponent } from './MovContaCorrente.component';

describe('MovContaCorrenteComponent', () => {
  let component: MovContaCorrenteComponent;
  let fixture: ComponentFixture<MovContaCorrenteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MovContaCorrenteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MovContaCorrenteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
