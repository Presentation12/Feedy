import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddlembreteComponent } from './addlembrete.component';

describe('AddlembreteComponent', () => {
  let component: AddlembreteComponent;
  let fixture: ComponentFixture<AddlembreteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddlembreteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddlembreteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
