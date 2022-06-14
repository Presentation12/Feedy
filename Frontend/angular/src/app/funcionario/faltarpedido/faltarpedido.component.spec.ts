import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FaltarpedidoComponent } from './faltarpedido.component';

describe('FaltarpedidoComponent', () => {
  let component: FaltarpedidoComponent;
  let fixture: ComponentFixture<FaltarpedidoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FaltarpedidoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FaltarpedidoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
