/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PefinRefinComponent } from './PefinRefin.component';

describe('PefinRefinComponent', () => {
  let component: PefinRefinComponent;
  let fixture: ComponentFixture<PefinRefinComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PefinRefinComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PefinRefinComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
