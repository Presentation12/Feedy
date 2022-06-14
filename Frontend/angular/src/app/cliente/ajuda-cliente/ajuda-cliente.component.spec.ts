import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AjudaClienteComponent } from './ajuda-cliente.component';

describe('AjudaClienteComponent', () => {
  let component: AjudaClienteComponent;
  let fixture: ComponentFixture<AjudaClienteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AjudaClienteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AjudaClienteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
