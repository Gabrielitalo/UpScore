/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { AnotacoesSPCComponent } from './AnotacoesSPC.component';

describe('AnotacoesSPCComponent', () => {
  let component: AnotacoesSPCComponent;
  let fixture: ComponentFixture<AnotacoesSPCComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AnotacoesSPCComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AnotacoesSPCComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
