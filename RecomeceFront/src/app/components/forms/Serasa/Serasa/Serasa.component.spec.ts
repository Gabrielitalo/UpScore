/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SerasaComponent } from './Serasa.component';

describe('SerasaComponent', () => {
  let component: SerasaComponent;
  let fixture: ComponentFixture<SerasaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SerasaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SerasaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
